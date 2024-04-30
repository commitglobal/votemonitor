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
    public bool IsDeleted { get; private set; }

    internal QuickReportAttachment(
        Guid id,
        Guid electionRoundId,
        Guid monitoringObserverId,
        Guid quickReportId,
        string fileName,
        string filePath,
        string mimeType) : base(id)
    {
        ElectionRoundId = electionRoundId;
        MonitoringObserverId = monitoringObserverId;
        QuickReportId = quickReportId;
        FileName = fileName;
        MimeType = mimeType;
        FilePath = filePath;
        IsDeleted = false;

        var extension = FileName.Split('.').Last();
        var uploadedFileName = $"{Id}.{extension}";
        UploadedFileName = uploadedFileName;
    }

    public void Delete()
    {
        IsDeleted = true;
    }

    public static QuickReportAttachment Create(Guid id,
        Guid electionRoundId,
        Guid monitoringObserverId,
        Guid quickReportId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, monitoringObserverId, quickReportId, fileName, filePath, mimeType);

#pragma warning disable CS8618 // Required by Entity Framework

    internal QuickReportAttachment(string uploadedFileName, string filePath)
    {
        UploadedFileName = uploadedFileName;
        FilePath = filePath;
    }

#pragma warning restore CS8618
}
