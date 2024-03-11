using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Services.PushNotification.Contracts;

namespace Vote.Monitor.Core.Services.PushNotification.Firebase;

public class FirebasePushNotificationService : IPushNotificationService
{
    private readonly ILogger<FirebasePushNotificationService> _logger;

    public FirebasePushNotificationService(ILogger<FirebasePushNotificationService> logger)
    {
        _logger = logger;
    }

    public async Task<SendNotificationResult> SendNotificationAsync(List<string> userIdentifiers, string title, string body, CancellationToken ct = default)
    {
        try
        {
            int successCount = 0;
            int failedCount = 0;

            foreach (var batch in userIdentifiers.Chunk(500))
            {
                var messages = batch.Select(userIdentifier => new Message
                {
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Token = userIdentifier
                });

                var response = await FirebaseMessaging.DefaultInstance.SendEachAsync(messages, ct);
                successCount += response.SuccessCount;
                failedCount += response.FailureCount;
                _logger.LogInformation("Batch notifications sent. {@response}", response);
            }

            return new SendNotificationResult.Ok(successCount, failedCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send notification");
            return new SendNotificationResult.Failed();
        }
    }
}
