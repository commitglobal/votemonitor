namespace Vote.Monitor.Feature.PollingStation.GetPollingStationsTagValues;
internal class TagValuesRequest
{
    public required string SelectTag { get; set; }
    public required string Filter { get; set; }
}
