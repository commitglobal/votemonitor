using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

public class MonitoringNgo : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid NgoId { get; private set; }
    public Ngo Ngo { get; private set; }
    public Guid FormStationsVersion { get; private set; }
    public virtual List<MonitoringObserver> MonitoringObservers { get; internal set; } = [];

    public MonitoringNgoStatus Status { get; private set; }

    internal MonitoringNgo(ElectionRound electionRound, Ngo ngo) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Ngo = ngo;
        NgoId = ngo.Id;
        Status = MonitoringNgoStatus.Active;
    }

    public virtual MonitoringObserver? AddMonitoringObserver(Observer observer)
    {
        if (MonitoringObservers.Any(x => x.ObserverId == observer.Id))
        {
            return null;
        }

        var monitoringObserver = new MonitoringObserver(this, observer);
        MonitoringObservers.Add(monitoringObserver);

        return monitoringObserver;
    }

    public bool IsObserverMonitoring(Observer observer)
    {
        return MonitoringObservers.Any(x => x.Id == observer.Id);
    }

    public virtual void ActivateMonitoringObserver(Guid monitoringObserverId)
    {
        var monitoringObserver = MonitoringObservers.First(x => x.Id == monitoringObserverId);
        monitoringObserver.Activate();
    }

    public virtual void SuspendMonitoringObserver(Guid monitoringObserverId)
    {
        var monitoringObserver = MonitoringObservers.First(x => x.Id == monitoringObserverId);
        monitoringObserver.Suspend();
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
