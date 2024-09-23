namespace Feature.Locations.FetchLevels;

public class Response
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public List<LevelNode> Nodes { get; set; } = [];
}
