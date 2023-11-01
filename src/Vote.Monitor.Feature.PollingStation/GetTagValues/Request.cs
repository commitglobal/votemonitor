namespace Vote.Monitor.Feature.PollingStation.GetTagValues;
internal class Request
{
    public required string SelectTag { get; set; }
    public Dictionary<string, string>? Filter { get; set; }
}
