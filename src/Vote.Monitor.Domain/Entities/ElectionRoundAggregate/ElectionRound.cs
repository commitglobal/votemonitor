namespace Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

public class ElectionRound : AuditableBaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    internal ElectionRound()
    {

    }
#pragma warning restore CS8618

    private readonly List<MonitoringNGO> _monitoringNgos = new();
    private readonly List<MonitoringObserver> _monitoringObservers = new();

    public string Title { get; private set; }
    public string EnglishTitle { get; private set; }
    public DateOnly StartDate { get; private set; }
    public ElectionRoundStatus Status { get; private set; } = ElectionRoundStatus.NotStarted;
    public Guid CountryId { get; private set; }
    public Country Country { get; private set; }

    public virtual IReadOnlyList<MonitoringNGO> MonitoringNgos => _monitoringNgos.ToList().AsReadOnly();
    public virtual IReadOnlyList<MonitoringObserver> MonitoringObservers => _monitoringObservers.ToList().AsReadOnly();

    public ElectionRound(Guid countryId,
        string title,
        string englishTitle,
        DateOnly startDate,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        Title = title;
        EnglishTitle = englishTitle;
        StartDate = startDate;
        CountryId = countryId;
        Status = ElectionRoundStatus.NotStarted;
    }

    public virtual void UpdateDetails(Guid countryId, string title, string englishTitle, DateOnly startDate)
    {
        Title = title;
        EnglishTitle = englishTitle;
        StartDate = startDate;
        CountryId = countryId;
    }

    public virtual void Start()
    {
        // todo: add invariants
        Status = ElectionRoundStatus.Started;
    }

    public virtual void Unstart()
    {
        // todo: add invariants
        Status = ElectionRoundStatus.NotStarted;
    }

    public virtual void Archive()
    {
        // todo: add invariants
        Status = ElectionRoundStatus.Archived;
    }

    public virtual void Unarchive()
    {
        // todo: add invariants
        Status = ElectionRoundStatus.NotStarted;
    }

    public virtual void AddMonitoringNgo(Guid ngoId)
    {
        var monitoringNgo = new MonitoringNGO(Id, ngoId);
        if (!_monitoringNgos.Contains(monitoringNgo))
        {
            _monitoringNgos.Add(monitoringNgo);
        }
    }
    public virtual void AddMonitoringObserver(Guid observerId, Guid invitingNgoId)
    {
        var monitoringObserver = new MonitoringObserver(Id, invitingNgoId, observerId);
        if (!_monitoringObservers.Contains(monitoringObserver))
        {
            _monitoringObservers.Add(monitoringObserver);
        }
    }

    public virtual void RemoveMonitoringNgo(Guid ngoId)
    {
        var ngo = _monitoringNgos.First(x => x.NgoId == ngoId);
        _monitoringNgos.Remove(ngo);
    }

    public virtual void RemoveMonitoringObserver(Guid observerId)
    {
        var monitoringObserver = _monitoringObservers.First(x => x.ObserverId == observerId);
        _monitoringObservers.Remove(monitoringObserver);
    }
}
