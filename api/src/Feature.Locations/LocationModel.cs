namespace Feature.Locations;
public class LocationModel
{
    public Guid Id { get; set; }
    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public string Level4 { get; set; }
    public string Level5 { get; set; }
    public int DisplayOrder { get; set; }
    public Dictionary<string, string> Tags { get; set; }
}
