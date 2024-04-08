namespace Feature.MonitoringObservers.AddObserver;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid ObserverId { get; set; }

    public Guid MonitoringNgoId { get; set; }
}
