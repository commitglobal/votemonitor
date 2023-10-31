namespace Vote.Monitor.Feature.PollingStation.GetPollingStationsTagValues;
internal class TagValuesRequest
{
    public required string SelectTag { get; set; }
    public Dictionary<string, string>? Filter { get; set; }
}
