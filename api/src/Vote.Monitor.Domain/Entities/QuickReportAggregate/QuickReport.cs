using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.QuickReportAggregate;

public class QuickReport : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public QuickReportLocationType QuickReportLocationType { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid? PollingStationId { private set; get; }
    public PollingStation? PollingStation { get; private set; }
    public string? PollingStationDetails { get; private set; }

    public static QuickReport Create(Guid id,
        Guid electionRoundId,
        Guid monitoringObserverId,
        string title,
        string description,
        QuickReportLocationType locationType,
        Guid? pollingStationId,
        string? pollingStationDetails)
    {
        return new QuickReport(id, electionRoundId, monitoringObserverId, locationType, title,
            description, pollingStationId, pollingStationDetails);
    }

    private QuickReport(Guid id,
        Guid electionRoundId,
        Guid monitoringObserverId,
        QuickReportLocationType quickReportLocationType,
        string title,
        string description,
        Guid? pollingStationId,
        string? pollingStationDetails) : base(id)
    {
        ElectionRoundId = electionRoundId;
        MonitoringObserverId = monitoringObserverId;
        QuickReportLocationType = quickReportLocationType;
        Title = title;
        Description = description;
        PollingStationId = pollingStationId;
        PollingStationDetails = pollingStationDetails;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal QuickReport()
    {
    }
#pragma warning restore CS8618

    public void Update(string title,
        string description,
        QuickReportLocationType locationType,
        Guid? pollingStationId,
        string? pollingStationDetails)
    {
        Title = title;
        Description = description;
        PollingStationId = pollingStationId;
        PollingStationDetails = pollingStationDetails;
    }
}
