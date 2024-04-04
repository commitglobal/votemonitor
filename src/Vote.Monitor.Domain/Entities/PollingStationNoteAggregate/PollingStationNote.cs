using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationNoteAggregate;

public class PollingStationNote : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid PollingStationId { get; private set; }
    public PollingStation PollingStation { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public string Text { get; private set; }
    
    public PollingStationNote(Guid electionRoundId,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        string text) : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;
        PollingStation = pollingStation;
        PollingStationId = pollingStation.Id;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        Text = text;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal PollingStationNote()
    {
    }
#pragma warning restore CS8618
    public void UpdateText(string text)
    {
        Text = text;
    }
}
