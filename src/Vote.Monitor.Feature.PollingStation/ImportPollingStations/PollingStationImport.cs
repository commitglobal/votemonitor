namespace Vote.Monitor.Feature.PollingStation.ImportPollingStations;
internal class PollingStationImport
{
    public int DisplayOrder { get; set; }
    public string Address { get; set; }
    public Dictionary<string,string> Tags { get; set; }
}
