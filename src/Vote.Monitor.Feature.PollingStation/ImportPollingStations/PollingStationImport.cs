namespace Vote.Monitor.Feature.PollingStation.ImportPollingStations;
internal class PollingStationImport
{
    public int DisplayOrder { get; set; }
    public required string   Address { get; set; }
    public Dictionary<string, string> Tags { get; set; } = new();
}
