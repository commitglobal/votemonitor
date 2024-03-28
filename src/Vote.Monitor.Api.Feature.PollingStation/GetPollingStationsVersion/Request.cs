namespace Vote.Monitor.Api.Feature.PollingStation.GetPollingStationsVersion;

public class Request
{
    public required Guid ElectionRoundId { get; set; }
}
