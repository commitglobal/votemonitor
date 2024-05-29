using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

public class QuickReportAttachment : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid QuickReportId { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public string FileName { get; private set; }
    public string UploadedFileName { get; private set; }
    public string FilePath { get; private set; }
    public string MimeType { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsDeleted { get; private set; }

    private QuickReportAttachment(
        Guid id,
        Guid electionRoundId,
        Guid monitoringObserverId,
        Guid quickReportId,
        string fileName,
        string filePath,
        string mimeType,
        bool isCompleted) : base(id)
    {
        ElectionRoundId = electionRoundId;
        MonitoringObserverId = monitoringObserverId;
        QuickReportId = quickReportId;
        FileName = fileName;
        MimeType = mimeType;
        FilePath = filePath;
        IsDeleted = false;
        IsCompleted = isCompleted;

        var extension = FileName.Split('.').Last();
        var uploadedFileName = $"{Id}.{extension}";
        UploadedFileName = uploadedFileName;
    }

    public void Delete()
    {
        IsDeleted = true;
    }

    public void Complete()
    {
        IsCompleted = true;
    }

    public static QuickReportAttachment Create(Guid id,
        Guid electionRoundId,
        Guid monitoringObserverId,
        Guid quickReportId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, monitoringObserverId, quickReportId, fileName, filePath, mimeType, true);

    public static QuickReportAttachment CreateV2(Guid id,
        Guid electionRoundId,
        Guid monitoringObserverId,
        Guid quickReportId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, monitoringObserverId, quickReportId, fileName, filePath, mimeType, false);

#pragma warning disable CS8618 // Required by Entity Framework

    internal QuickReportAttachment()
    {
    }

#pragma warning restore CS8618
}
