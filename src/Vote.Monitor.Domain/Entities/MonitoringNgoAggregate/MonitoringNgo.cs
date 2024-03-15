using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

public class MonitoringNgo : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid NgoId { get; private set; }
    public Ngo Ngo { get; private set; }

    public virtual List<MonitoringObserver>? MonitoringObservers { get; internal set; }

    public MonitoringNgoStatus Status { get; private set; }

    internal MonitoringNgo(ElectionRound electionRound, Ngo ngo, ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Ngo = ngo;
        NgoId = ngo.Id;
        Status = MonitoringNgoStatus.Active;
    }

    public virtual MonitoringObserver? AddMonitoringObserver(Observer observer, ITimeProvider timeProvider)
    {
        MonitoringObservers ??= new List<MonitoringObserver>();

        if (MonitoringObservers.Any(x => x.ObserverId == observer.Id))
        {
            return null;
        }

        var monitoringObserver = new MonitoringObserver( this, observer, timeProvider);
        MonitoringObservers.Add(monitoringObserver);

        return monitoringObserver;
    }

    public bool IsObserverMonitoring(Observer observer)
    {
        MonitoringObservers ??= new List<MonitoringObserver>();

        return MonitoringObservers.Any(x => x.Id == observer.Id);
    }

    public virtual void ActivateMonitoringObserver(Observer observer)
    {
        MonitoringObservers ??= new List<MonitoringObserver>();

        var monitoringObserver = MonitoringObservers.First(x => x.ObserverId == observer.Id);
        monitoringObserver.Activate();
    }

    public virtual void SuspendMonitoringObserver(Observer observer)
    {
        MonitoringObservers ??= new List<MonitoringObserver>();

        var monitoringObserver = MonitoringObservers.First(x => x.ObserverId == observer.Id);
        monitoringObserver.Suspend();
    }

    public virtual void RemoveMonitoringObserver(Observer observer)
    {
        MonitoringObservers ??= new List<MonitoringObserver>();

        var monitoringObserver = MonitoringObservers.First(x => x.ObserverId == observer.Id);
        MonitoringObservers.Remove(monitoringObserver);
    }

    public void Activate()
    {
        Status = MonitoringNgoStatus.Active;
    }

    public void Suspend()
    {
        Status = MonitoringNgoStatus.Suspended;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private MonitoringNgo()
    {

    }
#pragma warning restore CS8618
}
