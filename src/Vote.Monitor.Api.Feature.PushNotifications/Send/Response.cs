namespace Vote.Monitor.Api.Feature.PushNotifications.Send;

public record Response
{
    public required int SuccessCount { get; init; }
    public required int FailedCount { get; init; }
}
