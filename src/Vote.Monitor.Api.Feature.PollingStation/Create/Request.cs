namespace Vote.Monitor.Api.Feature.PollingStation.Create;
public class Request
{
    public Guid ElectionRoundId { get; set; }
    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public string Level4 { get; set; }
    public string Level5 { get; set; }
    public string Number { get; set; }
    public int DisplayOrder { get; set; }
    public string Address { get; set; }
    public Dictionary<string, string> Tags { get; set; }
}
