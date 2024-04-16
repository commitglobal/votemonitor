namespace Vote.Monitor.Api.Feature.PollingStation.Get;

public class Request
{
    public required Guid ElectionRoundId { get; set; }
    public required Guid Id { get; set; }
}
