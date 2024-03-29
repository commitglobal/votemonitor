namespace Vote.Monitor.Api.Feature.PollingStation.FetchAllV2;

public class Response
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public Dictionary<string, LocationNode> Nodes { get; set; }
}
