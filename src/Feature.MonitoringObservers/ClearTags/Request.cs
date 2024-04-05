namespace Feature.MonitoringObservers.ClearTags;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid MonitoringNgoId { get; set; }
    public List<Guid> MonitoringObserverIds { get; set; }
}
