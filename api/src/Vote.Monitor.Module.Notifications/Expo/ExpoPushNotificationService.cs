using System.Text.RegularExpressions;
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
    ILogger<ExpoPushNotificationService> logger)
    : IPushNotificationService
{
    private readonly ExpoOptions _options = options.Value;

    public async Task<SendNotificationResult> SendNotificationAsync(List<string> userIdentifiers, string title, string body, CancellationToken ct = default)
    {
        try
        {
            var successCount = 0;
            var failedCount = 0;

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<VoteMonitorContext>();

                foreach (var identifiersBatch in userIdentifiers.Chunk(_options.BatchSize))
                {
                    var expoIdentifiers = identifiersBatch.Where(IsExpoPushToken).ToList();
                    if (!expoIdentifiers.Any())
                    {
                        continue;
                    }

                    var request = new PushTicketRequest
                    {
                        PushTo = expoIdentifiers,
                        PushTitle = title,
                        PushBody = body,
                        PushChannelId = _options.ChannelId,
                        PushTTL = _options.TtlSeconds,
                        PushPriority = _options.Priority
                    };

                    var response = await expoApi.SendNotificationAsync(request).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content.PushTicketErrors != null && response.Content.PushTicketErrors.Any())
                        {
                            logger.LogError("Error received when sending push notification {@request} {@response}",
                                request, response);

                            failedCount += identifiersBatch.Length;
                            continue;
                        }

                        var serializedData = serializerService.Serialize(response.Content);

                        context.NotificationStubs.Add(NotificationStub.CreateExpoNotificationStub(serializedData));
                        context.SaveChanges();
                        successCount += identifiersBatch.Length;
                    }
                    else
                    {
                        logger.LogError("Error received in expo response {@request} {@response}", request, response);

                        failedCount += identifiersBatch.Length;
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
               Regex.IsMatch(token, @"^[a-z\d]{8}-[a-z\d]{4}-[a-z\d]{4}-[a-z\d]{4}-[a-z\d]{12}$", RegexOptions.IgnoreCase);
    }
}
