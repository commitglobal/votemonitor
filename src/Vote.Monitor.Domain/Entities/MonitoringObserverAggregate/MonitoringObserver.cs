using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ObserverAggregate;

namespace Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

public class MonitoringObserver : AuditableBaseEntity, IAggregateRoot
{
    public Guid ObserverId { get; private set; }
    public Observer Observer { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public MonitoringNgo MonitoringNgo { get; private set; }
    public MonitoringObserverStatus Status { get; private set; }

    public string[] Tags { get; private set; }

    private MonitoringObserver(Guid monitoringNgoId, Guid observerId)
        : base(Guid.NewGuid())
    {
        MonitoringNgoId = monitoringNgoId;
        ObserverId = observerId;
        Tags = [];
        Status = MonitoringObserverStatus.Pending;
    }
    private MonitoringObserver(Guid monitoringNgoId, Observer observer)
        : base(Guid.NewGuid())
    {
        MonitoringNgoId = monitoringNgoId;
        ObserverId = observer.Id;
        Tags = [];
        Status = MonitoringObserverStatus.Pending;
    }

    internal MonitoringObserver(MonitoringNgo monitoringNgo, Observer observer)
        : base(Guid.NewGuid())
    {
        MonitoringNgoId = monitoringNgo.Id;
        MonitoringNgo = monitoringNgo;
        ObserverId = observer.Id;
        Observer = observer;
        Tags = [];
        Status = MonitoringObserverStatus.Pending;
    }

    internal MonitoringObserver(Guid id, MonitoringNgo monitoringNgo, Observer observer, string[] tags)
        : base(id)
    {
        MonitoringNgoId = monitoringNgo.Id;
        MonitoringNgo = monitoringNgo;
        ObserverId = observer.Id;
        Observer = observer;
        Tags = tags.Distinct().ToArray();
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

    public static MonitoringObserver Create(Guid monitoringNgoId, Guid observerId)
    {
        return new MonitoringObserver(monitoringNgoId, observerId);
    }

    public static MonitoringObserver Create(Guid monitoringNgoId, Observer observer)
    {
        return new MonitoringObserver(monitoringNgoId, observer);
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
