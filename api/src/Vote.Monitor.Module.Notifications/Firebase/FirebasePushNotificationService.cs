using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vote.Monitor.Module.Notifications.Contracts;

namespace Vote.Monitor.Module.Notifications.Firebase;

public class FirebasePushNotificationService : IPushNotificationService
{
    private readonly ILogger<FirebasePushNotificationService> _logger;
    private readonly FirebaseOptions _options;

    public FirebasePushNotificationService(IOptions<FirebaseOptions> options, ILogger<FirebasePushNotificationService> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<SendNotificationResult> SendNotificationAsync(List<string> userIdentifiers, string title, string body, CancellationToken ct = default)
    {
        try
        {
            var successCount = 0;
            var failedCount = 0;

            var batchedMessages = userIdentifiers
                .Select(identifier => new Message
                {
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Token = identifier
                }).Chunk(_options.BatchSize);

            foreach (var batch in batchedMessages)
            {
                var response = await FirebaseMessaging.DefaultInstance.SendEachAsync(batch, ct);

                successCount += response.SuccessCount;
                failedCount += response.FailureCount;

                _logger.LogInformation("Batch notifications sent. {@response}", response);
            }

            return new SendNotificationResult.Ok(successCount, failedCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send notification");
            SentrySdk.CaptureException(e);
            return new SendNotificationResult.Failed();
        }
    }
}
