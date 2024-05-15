namespace Vote.Monitor.Api.Feature.Notifications.Send;

public record Response
{
    public required int SuccessCount { get; init; }
    public required int FailedCount { get; init; }
    public string Status { get; set; }
}
