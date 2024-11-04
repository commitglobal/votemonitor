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

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string EnglishTitle { get; private set; }
    public DateOnly StartDate { get; private set; }
    public ElectionRoundStatus Status { get; private set; } = ElectionRoundStatus.NotStarted;
    public Guid CountryId { get; private set; }
    public Country Country { get; private set; }

    public virtual IReadOnlyList<MonitoringNgo> MonitoringNgos => _monitoringNgos.ToList().AsReadOnly();
    public Guid PollingStationsVersion { get; private set; }
    public Guid LocationsVersion { get; private set; }

    public bool CitizenReportingEnabled { get; private set; }
    public Guid? MonitoringNgoForCitizenReportingId { get; private set; }
    public MonitoringNgo? MonitoringNgoForCitizenReporting { get; private set; }

    public ElectionRound(Guid countryId,
        string title,
        string englishTitle,
        DateOnly startDate)
    {
        Id = Guid.NewGuid();
        Title = title;
        EnglishTitle = englishTitle;
        StartDate = startDate;
        CountryId = countryId;
        Status = ElectionRoundStatus.NotStarted;
        PollingStationsVersion = Guid.NewGuid();
        LocationsVersion = Guid.NewGuid();
    }

    internal ElectionRound(Guid id,
        string title,
        string englishTitle,
        DateOnly startDate,
        Country country,
        List<MonitoringNgo> monitoringNgos)
    {
        Id = id;
        Title = title;
        EnglishTitle = englishTitle;
        StartDate = startDate;
        Country = country;
        CountryId = country.Id;
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

    public MonitoringNgo AddMonitoringNgo(Ngo ngo)
    {
        var monitoringNgo = new MonitoringNgo(this, ngo);
        return monitoringNgo;
    }

    public virtual void RemoveMonitoringNgo(MonitoringNgo monitoringNgo)
    {
        var ngo = _monitoringNgos.First(x => x.NgoId == monitoringNgo.NgoId);
        _monitoringNgos.Remove(ngo);
    }

    public void UpdatePollingStationsVersion()
    {
        PollingStationsVersion = Guid.NewGuid();
    }
    public void UpdateLocationsVersion()
    {
        LocationsVersion = Guid.NewGuid();
    }

    public void EnableCitizenReporting(MonitoringNgo monitoringNgo)
    {
        if (CitizenReportingEnabled)
        {
            throw new ArgumentException("Citizen reporting is already enabled");
        }

        CitizenReportingEnabled = true;
        MonitoringNgoForCitizenReporting = monitoringNgo;
    }

    public void DisableCitizenReporting()
    {
        CitizenReportingEnabled = false;
    }
}