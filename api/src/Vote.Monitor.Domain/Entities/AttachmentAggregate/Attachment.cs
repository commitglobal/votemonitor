using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.AttachmentAggregate;

public class Attachment : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid PollingStationId { get; private set; }
    public PollingStation PollingStation { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }

    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public Guid QuestionId { get; private set; }
    public string FileName { get; private set; }
    public string UploadedFileName { get; private set; }
    public string FilePath { get; private set; }
    public string MimeType { get; private set; }

    public bool IsDeleted { get; private set; }
    public bool IsCompleted { get; private set; }

    private Attachment(Guid id,
        Guid electionRoundId,
        Guid pollingStationId,
        Guid monitoringObserverId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType,
        bool? isCompleted) : base(id)
    {
        ElectionRoundId = electionRoundId;
        PollingStationId = pollingStationId;
        MonitoringObserverId = monitoringObserverId;
        FormId = formId;
        QuestionId = questionId;
        FileName = fileName;
        FilePath = filePath;
        MimeType = mimeType;
        IsDeleted = false;
        
        if(isCompleted.HasValue)
        {
            IsCompleted = isCompleted.Value;
        }

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

#pragma warning disable CS8618 // Required by Entity Framework

    internal Attachment()
    {
    }
#pragma warning restore CS8618
    public static Attachment Create(Guid id,
        Guid electionRoundId,
        Guid pollingStationId,
        Guid monitoringObserverId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, pollingStationId, monitoringObserverId, formId, questionId, fileName, filePath, mimeType, true);

    public static Attachment CreateV2(Guid id,
        Guid electionRoundId,
        Guid pollingStationId,
        Guid monitoringObserverId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, pollingStationId, monitoringObserverId, formId, questionId, fileName, filePath, mimeType, false);
}
