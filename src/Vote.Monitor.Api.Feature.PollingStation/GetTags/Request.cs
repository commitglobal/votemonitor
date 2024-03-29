namespace Vote.Monitor.Api.Feature.PollingStation.GetTags;

public class Request
{
    public required Guid ElectionRoundId { get; set; }
}
