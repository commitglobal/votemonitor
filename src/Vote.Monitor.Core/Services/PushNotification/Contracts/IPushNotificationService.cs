namespace Vote.Monitor.Core.Services.PushNotification.Contracts;

public interface IPushNotificationService
{
    Task<SendNotificationResult> SendNotificationAsync(List<string> userIdentifiers, string title, string body, CancellationToken ct = default);
}
