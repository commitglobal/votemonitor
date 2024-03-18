using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationAttachmentAggregate;

public class PollingStationAttachment : BaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid PollingStationId { get; private set; }
    public PollingStation PollingStation { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string MimeType { get; private set; }
    public DateTime Timestamp { get; private set; }

    public PollingStationAttachment(ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        string fileName,
        string filePath,
        string mimeType,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        PollingStationId = pollingStation.Id;
        PollingStation = pollingStation;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        Timestamp = timeProvider.UtcNow;
        FileName = fileName;
        FilePath = filePath;
        MimeType = mimeType;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal PollingStationAttachment()
    {
    }
#pragma warning restore CS8618
}
