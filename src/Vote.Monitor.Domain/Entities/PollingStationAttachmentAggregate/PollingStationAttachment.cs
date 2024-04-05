using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationAttachmentAggregate;

public class PollingStationAttachment : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid PollingStationId { get; private set; }
    public PollingStation PollingStation { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public string FileName { get; private set; }
    public string UploadedFileName { get; private set; }
    public string FilePath { get; private set; }
    public string MimeType { get; private set; }
    public bool IsDeleted { get; private set; }

    public PollingStationAttachment(Guid electionRoundId,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        string fileName,
        string filePath,
        string mimeType) : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;
        PollingStationId = pollingStation.Id;
        PollingStation = pollingStation;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        FileName = fileName;
        FilePath = filePath;
        MimeType = mimeType;
        IsDeleted = false;

        var extension = FileName.Split('.').Last();
        var uploadedFileName = $"{Id}.{extension}";
        UploadedFileName = uploadedFileName;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal PollingStationAttachment()
    {
    }
#pragma warning restore CS8618
    public void Delete()
    {
        IsDeleted = true;
    }
}
