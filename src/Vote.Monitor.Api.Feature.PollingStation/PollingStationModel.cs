namespace Vote.Monitor.Api.Feature.PollingStation;
public class PollingStationModel
{
    public required Guid Id { get; set; }
    public required string Level1 { get; set; }
    public required string Level2 { get; set; }
    public required string Level3 { get; set; }
    public required string Level4 { get; set; }
    public required string Level5 { get; set; }
    public required string Number { get; set; }
    public required string Address { get; set; }
    public required int DisplayOrder { get; set; }
    public Dictionary<string, string> Tags { get; set; }
}
