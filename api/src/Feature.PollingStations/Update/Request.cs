namespace Feature.PollingStations.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid Id { get; set; }
    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public string Level4 { get; set; }
    public string Level5 { get; set; }
    public string Number { get; set; }
    public int DisplayOrder { get; set; }
    public string Address { get; set; }
    public Dictionary<string, string> Tags { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
