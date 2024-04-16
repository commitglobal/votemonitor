using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.NotificationStubAggregate;
using Vote.Monitor.Module.Notifications.Contracts;
using Vote.Monitor.Module.Notifications.Expo.Models;

namespace Vote.Monitor.Module.Notifications.Expo;

public class ExpoPushNotificationService(
    VoteMonitorContext context,
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

            foreach (var identifiersBatch in userIdentifiers.Chunk(_options.BatchSize))
            {
                foreach (var userIdentifier in identifiersBatch)
                {
                    if (!IsExpoPushToken(userIdentifier))
                    {
                        continue;
                    }

                    var request = new PushTicketRequest
                    {
                        PushTo = identifiersBatch.ToList(),
                        PushTitle = title,
                        PushBody = body
                    };

                    var response = await expoApi.SendNotificationAsync(request).ConfigureAwait(false);

                    if (response.ErrorInformation != null && response.ErrorInformation.Any())
                    {
                        logger.LogError("Error received when sending push notification {@request} {@error}", request, response.ErrorInformation);

                        failedCount += identifiersBatch.Length;
                        continue;
                    }

                    var serializedData = serializerService.Serialize(response);

                    context.NotificationStubs.Add(NotificationStub.CreateExpoNotificationStub(serializedData));
                    successCount += identifiersBatch.Length;

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
