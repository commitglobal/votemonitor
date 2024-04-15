using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Services.PushNotification.Contracts;

namespace Vote.Monitor.Core.Services.PushNotification.Noop;

public class NoopPushNotificationService(ILogger<NoopPushNotificationService> logger) : IPushNotificationService
{
    public async Task<SendNotificationResult> SendNotificationAsync(List<string> userIdentifiers, string title, string body, CancellationToken ct = default)
    {
        await Task.Delay(200, ct);

        logger.LogInformation("Sending notification {@users} {title} {body}", userIdentifiers, title, body);

        return new SendNotificationResult.Ok(userIdentifiers.Count, 0);
    }
}
