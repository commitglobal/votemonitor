using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.NotificationStubAggregate;
using Vote.Monitor.Module.Notifications.Contracts;
using Vote.Monitor.Module.Notifications.Expo.Models;

namespace Vote.Monitor.Module.Notifications.Expo;

public class ExpoPushNotificationService(
    IServiceScopeFactory serviceScopeFactory,
    IExpoApi expoApi,
    ISerializerService serializerService,
    IOptions<ExpoOptions> options,
    IPushNotificationRateLimiter limiter,
    ILogger<ExpoPushNotificationService> logger)
    : IPushNotificationService
{
    private readonly ExpoOptions _options = options.Value;
    private readonly RateLimiter _rateLimiter = limiter.Limiter;

    public async Task SendNotificationAsync(List<string> userIdentifiers,
        string title,
        string body,
        CancellationToken ct = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VoteMonitorContext>();

        var tokensBatches = userIdentifiers.Where(IsExpoPushToken).Chunk(_options.BatchSize);
        var notificationsBatches = new Queue<string[]>(tokensBatches);
        while (true)
        {
            if (notificationsBatches.Count == 0)
            {
                break;
            }

            var lease = await _rateLimiter.AcquireAsync(notificationsBatches.Peek().Length, ct);
            if (lease.IsAcquired)
            {
                var sendTo = notificationsBatches.Dequeue().ToList();
                await SendAsync(sendTo, title, context, ct);
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
        }
    }

    private async Task SendAsync(List<string> to, string title, VoteMonitorContext context, CancellationToken ct)
    {
        logger.LogInformation("Sending notifications to {to} observers", to.Count);
        var request = new PushTicketRequest
        {
            PushTo = to,
            PushTitle = title,
            PushChannelId = _options.ChannelId,
            PushTTL = _options.TtlSeconds,
            PushPriority = _options.Priority
        };

        var response = await expoApi.SendNotificationAsync(request).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            if (response.Content.PushTicketErrors != null && response.Content.PushTicketErrors.Any())
            {
                logger.LogError("Error received when sending push notification {@response} {@request}",
                    response, request);
                return;
            }

            var serializedData = serializerService.Serialize(response.Content);

            context.NotificationStubs.Add(NotificationStub.CreateExpoNotificationStub(serializedData));
            await context.SaveChangesAsync(ct);
        }
        else
        {
            logger.LogError("Error received in expo response {@request} {@response}", request, response);
        }
    }

    private static bool IsExpoPushToken(string token)
    {
        return (token.StartsWith("ExponentPushToken[") || token.StartsWith("ExpoPushToken[")) &&
               token.EndsWith("]") ||
               Regex.IsMatch(token, @"^[a-z\d]{8}-[a-z\d]{4}-[a-z\d]{4}-[a-z\d]{4}-[a-z\d]{12}$",
                   RegexOptions.IgnoreCase);
    }
}