using Vote.Monitor.Domain.Entities.EmergencyAttachmentAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.EmergencyAggregate;

public class Emergency : BaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public DateTime ReportedAt { get; private set; }

    public LocationType LocationType { get; private set; }

    public Guid? PollingStationId { get; private set; }
    public PollingStation? PollingStation { get; private set; }

    public string? PollingStationDescription { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }

    private List<EmergencyAttachment> _attachments = new();
    public virtual IReadOnlyList<EmergencyAttachment> Attachments => _attachments.ToList().AsReadOnly();

    private Emergency(ElectionRound electionRound,
        MonitoringObserver monitoringObserver,
        LocationType locationType,
        DateTime reportedAt,
        PollingStation? pollingStation,
        string? pollingStationDescription,
        string title,
        string description,
        List<EmergencyAttachment> attachments,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        ElectionRoundId = electionRound.Id;
        ElectionRound = electionRound;
        MonitoringObserverId = monitoringObserver.Id;
        MonitoringObserver = monitoringObserver;
        ReportedAt = reportedAt;
        LocationType = locationType;
        PollingStation = pollingStation;
        PollingStationId = pollingStation?.Id;
        PollingStationDescription = pollingStationDescription;
        Title = title;
        Description = description;
        _attachments = attachments;
    }

    private Emergency(ElectionRound electionRound,
        MonitoringObserver monitoringObserver,
        LocationType locationType,
        DateTime reportedAt,
        PollingStation? pollingStation,
        string title,
        string description,
        ITimeProvider timeProvider) : this(electionRound,
            monitoringObserver,
            locationType,
            reportedAt,
            pollingStation,
            pollingStationDescription: null,
            title,
            description,
            [],
            timeProvider)
    {

    }

    private Emergency(ElectionRound electionRound,
        MonitoringObserver monitoringObserver,
        LocationType locationType,
        DateTime reportedAt,
        string pollingStationDescription,
        string title,
        string description,
        ITimeProvider timeProvider) : this(electionRound,
        monitoringObserver,
        locationType,
        reportedAt,
        pollingStation: null,
        pollingStationDescription,
        title,
        description,
        [],
        timeProvider)
    {
    }

    private Emergency(ElectionRound electionRound,
        MonitoringObserver monitoringObserver,
        LocationType locationType,
        DateTime reportedAt,
        string title,
        string description,
        ITimeProvider timeProvider) : this(electionRound,
        monitoringObserver,
        locationType,
        reportedAt,
        pollingStation: null,
        pollingStationDescription: null,
        title,
        description,
        [],
        timeProvider)
    {
    }

    internal Emergency CreateVisitedPollingStationEmergency(ElectionRound electionRound,
        MonitoringObserver observer,
        PollingStation pollingStation,
        string title,
        string description,
        ITimeProvider timeProvider)
    {
        return new(electionRound,
            observer,
            LocationType.VisitedPollingStation,
            timeProvider.UtcNow,
            pollingStation,
            title,
            description,
            timeProvider);
    }

    internal Emergency CreateOtherPollingStationEmergency(ElectionRound electionRound,
        MonitoringObserver observer,
        string pollingStationDescription,
        string title,
        string description,
        ITimeProvider timeProvider)
    {
        return new(electionRound,
            observer,
            LocationType.OtherPollingStation,
            timeProvider.UtcNow,
            pollingStationDescription,
            title,
            description,
            timeProvider);
    }

    internal Emergency CreateNotRelatedToPollingStationEmergency(ElectionRound electionRound,
        MonitoringObserver observer,
        string title,
        string description,
        ITimeProvider timeProvider)
    {
        return new(electionRound,
            observer,
            LocationType.NotRelatedToPollingStation,
            timeProvider.UtcNow,
            title,
            description,
            timeProvider);
    }

#pragma warning disable CS8618 // Required by Entity Framework

    private Emergency()
    {

    }
#pragma warning restore CS8618
}
