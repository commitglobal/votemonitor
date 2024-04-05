namespace Feature.MonitoringObservers.Untag;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid MonitoringNgoId { get; set; }
    public Guid[] MonitoringObserverIds { get; set; } = [];
    public string[] Tags { get; set; } = [];
}
