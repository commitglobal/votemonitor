namespace SubmissionsFaker.Clients.PollingStations;

public class AllPollingStations
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public List<PollingStationNode> Nodes { get; set; } = [];
}
