namespace Vote.Monitor.Api.Feature.PollingStation.FetchAllV2;

public class LocationNode
{
    public string Name { get; set; }
    public LocationNode? Parent { get; set; }

    public Dictionary<string, PollingStationDetails> PollingStations { get; set; } = new();
}
