using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ObserverAggregate;

namespace Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

public class MonitoringObserver : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public MonitoringNgo MonitoringNgo { get; private set; }

    public Guid ObserverId { get; private set; }
    public Observer Observer { get; private set; }

    public MonitoringObserverStatus Status { get; private set; }

    public string[] Tags { get; private set; }

    private MonitoringObserver(Guid electionRoundId, Guid monitoringNgoId, Guid observerId, string[] tags)
        : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
        ObserverId = observerId;
        Tags = tags;
        Status = MonitoringObserverStatus.Pending;
    }

    public void Activate()
    {
        Status = MonitoringObserverStatus.Active;
    }

    public void Suspend()
    {
        Status = MonitoringObserverStatus.Suspended;
    }

    public static MonitoringObserver Create(Guid electionRoundId, Guid monitoringNgoId, Guid observerId, string[] tags)
    {
        return new MonitoringObserver(electionRoundId, monitoringNgoId, observerId, tags);
    }

    public void Update(MonitoringObserverStatus status, string[] tags)
    {
        Status = status;
        Tags = tags;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private MonitoringObserver()
    {

    }
#pragma warning restore CS8618
}
