namespace SubmissionsFaker.Clients.Locations;

public class AllLocations
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public List<LocationNode> Nodes { get; set; } = [];
}
