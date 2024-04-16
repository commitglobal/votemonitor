namespace Vote.Monitor.Module.Notifications.Contracts;

public interface IPushNotificationService
{
    Task<SendNotificationResult> SendNotificationAsync(List<string> userIdentifiers, string title, string body, CancellationToken ct = default);
}
