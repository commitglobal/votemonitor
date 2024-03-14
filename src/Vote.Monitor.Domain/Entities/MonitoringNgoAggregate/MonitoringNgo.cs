using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

public class MonitoringNgo : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid NgoId { get; private set; }
    public Ngo Ngo { get; private set; }

    private readonly List<MonitoringObserver> _monitoringObservers = new();
    public IReadOnlyList<MonitoringObserver> MonitoringObservers => _monitoringObservers.AsReadOnly();
    public MonitoringNgoStatus Status { get; private set; }

    internal MonitoringNgo(ElectionRound electionRound, Ngo ngo, ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Ngo = ngo;
        NgoId = ngo.Id;
        Status = MonitoringNgoStatus.Active;
    }

    public virtual void AddMonitoringObserver(Observer observer, ITimeProvider timeProvider)
    {
        if (_monitoringObservers.Any(x => x.Id == observer.Id))
        {
            return;
        }

        var monitoringObserver = new MonitoringObserver( this, observer, timeProvider);
        _monitoringObservers.Add(monitoringObserver);
    }

    public bool IsObserverMonitoring(Observer observer)
    {
        return _monitoringObservers.Any(x => x.Id == observer.Id);
    }

    public virtual void ActivateMonitoringObserver(Observer observer)
    {
        var monitoringObserver = _monitoringObservers.First(x => x.ObserverId == observer.Id);
        monitoringObserver.Activate();
    }

    public virtual void SuspendMonitoringObserver(Observer observer)
    {
        var monitoringObserver = _monitoringObservers.First(x => x.ObserverId == observer.Id);
        monitoringObserver.Suspend();
    }

    public virtual void RemoveMonitoringObserver(Observer observer)
    {
        var monitoringObserver = _monitoringObservers.First(x => x.ObserverId == observer.Id);
        _monitoringObservers.Remove(monitoringObserver);
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
