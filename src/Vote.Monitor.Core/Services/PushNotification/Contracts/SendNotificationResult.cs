namespace Vote.Monitor.Core.Services.PushNotification.Contracts;

public abstract record SendNotificationResult
{
    public record Ok(int SuccessCount, int FailedCount) : SendNotificationResult;
    public record Failed : SendNotificationResult;
    private SendNotificationResult(){}
}
