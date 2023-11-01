namespace Vote.Monitor.Feature.PollingStation.Create;
public class Request
{
    public required int DisplayOrder { get; set; }
    public required string Address { get; set; }
    public required Dictionary<string, string> Tags { get; set; }
}
