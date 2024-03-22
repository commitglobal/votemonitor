using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationNoteAggregate;

public class PollingStationNote : BaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid PollingStationId { get; private set; }
    public PollingStation PollingStation { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Text { get; private set; }

    public PollingStationNote(ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        string text,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        PollingStation = pollingStation;
        PollingStationId = pollingStation.Id;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        Timestamp = timeProvider.UtcNow;
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
