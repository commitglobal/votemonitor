using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

public class ElectionRound : AuditableBaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    internal ElectionRound()
    {

    }
#pragma warning restore CS8618

    private readonly List<MonitoringNgo> _monitoringNgos = new();

    public string Title { get; private set; }
    public string EnglishTitle { get; private set; }
    public DateOnly StartDate { get; private set; }
    public ElectionRoundStatus Status { get; private set; } = ElectionRoundStatus.NotStarted;
    public Guid CountryId { get; private set; }
    public Country Country { get; private set; }

    public virtual IReadOnlyList<MonitoringNgo> MonitoringNgos => _monitoringNgos.ToList().AsReadOnly();

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

    internal ElectionRound(Guid id,
        string title,
        string englishTitle,
        DateOnly startDate,
        Country country,
        List<MonitoringNgo> monitoringNgos,
        ITimeProvider timeProvider) : base(id, timeProvider)
    {
        Title = title;
        EnglishTitle = englishTitle;
        StartDate = startDate;
        Country = country;
        _monitoringNgos = monitoringNgos;
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

    public MonitoringNgo AddMonitoringNgo(Ngo ngo, ITimeProvider timeProvider)
    {
        var monitoringNgo = new MonitoringNgo(this, ngo, timeProvider);
        return monitoringNgo;
    }

    public virtual void RemoveMonitoringNgo(MonitoringNgo monitoringNgo)
    {
        var ngo = _monitoringNgos.First(x => x.NgoId == monitoringNgo.NgoId);
        _monitoringNgos.Remove(ngo);
    }
}
