using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

public class MonitoringObserver : AuditableBaseEntity, IAggregateRoot
{
    public Guid ObserverId { get; private set; }
    public Observer Observer { get; private set; }
    public Guid InviterNgoId { get; private set; }
    public MonitoringNgo InviterNgo { get; private set; }
    public MonitoringObserverStatus Status { get; private set; }

    internal MonitoringObserver(MonitoringNgo inviterNgo, Observer observer, ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        InviterNgoId = inviterNgo.Id;
        InviterNgo = inviterNgo;
        ObserverId = observer.Id;
        Observer = observer;

        Status = MonitoringObserverStatus.Active;
    }

    public void Activate()
    {
        Status = MonitoringObserverStatus.Active;
    }

    public void Suspend()
    {
        Status = MonitoringObserverStatus.Suspended;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    private MonitoringObserver()
    {

    }
#pragma warning restore CS8618
}
