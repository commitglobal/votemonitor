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

    public async Task<SendNotificationResult> SendNotificationAsync(List<string> userIdentifiers, string title,
        string body, CancellationToken ct = default)
    {
        try
        {
            var successCount = 0;
            var failedCount = 0;

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<VoteMonitorContext>();

                var identifiersBatchesQueue =
                    new Queue<string[]>(userIdentifiers.Where(IsExpoPushToken).Chunk(_options.BatchSize));

                while (true)
                {
                    if (!identifiersBatchesQueue.Any())
                    {
                        break;
                    }

                    var lease = await _rateLimiter.AcquireAsync(identifiersBatchesQueue.Peek().Length, ct);
                    if (lease.IsAcquired)
                    {
                        var identifiersBatch = identifiersBatchesQueue.Dequeue().ToList();

                        var request = new PushTicketRequest
                        {
                            PushTo = identifiersBatch,
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

                                failedCount += identifiersBatch.Count;
                                continue;
                            }

                            var serializedData = serializerService.Serialize(response.Content);

                            context.NotificationStubs.Add(NotificationStub.CreateExpoNotificationStub(serializedData));
                            await context.SaveChangesAsync(ct);
                            successCount += identifiersBatch.Count;
                        }
                        else
                        {
                            logger.LogError("Error received in expo response {@request} {@response}", request,
                                response);

                            failedCount += identifiersBatch.Count;
                        }
                    }
                    else
                    {
                        // wait 10 seconds
                        await Task.Delay(TimeSpan.FromSeconds(10), ct).ConfigureAwait(false);
                    }
                }
            }

            return new SendNotificationResult.Ok(successCount, failedCount);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send notification");
            SentrySdk.CaptureException(e);
            return new SendNotificationResult.Failed();
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