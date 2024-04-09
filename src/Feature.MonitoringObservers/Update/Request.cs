using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid MonitoringNgoId { get; set; }
    public Guid Id { get; set; }
    public string[] Tags { get; set; }
    public MonitoringObserverStatus Status { get; set; }
}
