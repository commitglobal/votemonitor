using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vote.Monitor.Module.Notifications.Contracts;

namespace Vote.Monitor.Module.Notifications.Firebase;

public class FirebasePushNotificationService(
    IOptions<FirebaseOptions> options,
    ILogger<FirebasePushNotificationService> logger)
    : IPushNotificationService
{
    private readonly FirebaseOptions _options = options.Value;

    public async Task SendNotificationAsync(List<string> userIdentifiers, string title, string body,
        CancellationToken ct = default)
    {
        var batchedMessages = userIdentifiers
            .Select(identifier => new Message
            {
                Notification = new Notification
                {
                    Title = title,
                },
                Token = identifier
            }).Chunk(_options.BatchSize);

        foreach (var batch in batchedMessages)
        {
            var response = await FirebaseMessaging.DefaultInstance.SendEachAsync(batch, ct);

            logger.LogInformation("Batch notifications sent. {@response}", response);
        }
    }
}