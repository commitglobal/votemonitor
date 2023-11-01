namespace Vote.Monitor.Feature.PollingStation.GetTagValues;
public class Request
{
    public required string SelectTag { get; set; }
    public Dictionary<string, string>? Filter { get; set; }
}
