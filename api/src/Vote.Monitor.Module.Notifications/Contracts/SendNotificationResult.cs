namespace Vote.Monitor.Module.Notifications.Contracts;

public abstract record SendNotificationResult
{
    public record Ok(int SuccessCount, int FailedCount) : SendNotificationResult;
    public record Failed : SendNotificationResult;
    private SendNotificationResult() { }
}
