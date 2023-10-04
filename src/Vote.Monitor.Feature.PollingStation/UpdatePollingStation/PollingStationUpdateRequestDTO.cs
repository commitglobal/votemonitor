namespace Vote.Monitor.Feature.PollingStation.UpdatePollingStation;
internal class PollingStationUpdateRequestDTO
{
    public int DisplayOrder { get; set; }
    public required string Address { get; set; }
    public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
}
