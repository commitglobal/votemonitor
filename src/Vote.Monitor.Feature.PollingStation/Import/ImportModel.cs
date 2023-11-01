namespace Vote.Monitor.Feature.PollingStation.Import;
public class ImportModel
{
    public required int DisplayOrder { get; set; }
    public required string Address { get; set; }
    public required Dictionary<string, string> Tags { get; set; }
}
