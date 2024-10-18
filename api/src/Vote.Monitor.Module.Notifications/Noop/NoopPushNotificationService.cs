using Microsoft.Extensions.Logging;
using Vote.Monitor.Module.Notifications.Contracts;

namespace Vote.Monitor.Module.Notifications.Noop;

public class NoopPushNotificationService(ILogger<NoopPushNotificationService> logger) : IPushNotificationService
{
    public async Task SendNotificationAsync(List<string> userIdentifiers, string title, string body,
        CancellationToken ct = default)
    {
        await Task.Delay(200, ct);

        logger.LogInformation("Sending notification {@users} {title} {body}", userIdentifiers, title, body);
    }
}