using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.AttachmentAggregate;

public class Attachment : IAggregateRoot
{
    public Guid Id { get; private set; }

    [Obsolete("Will be removed in future version")]
    public Guid ElectionRoundId { get; private set; }

    [Obsolete("Will be removed in future version")]
    public Guid PollingStationId { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }

    [Obsolete("Will be removed in future version")]
    public Guid FormId { get; private set; }
    public Guid SubmissionId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string FileName { get; private set; }
    public string UploadedFileName { get; private set; }
    public string FilePath { get; private set; }
    public string MimeType { get; private set; }

    public bool IsDeleted { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    [Obsolete("Will be removed in future version")]
    private Attachment(Guid id,
        Guid electionRoundId,
        Guid pollingStationId,
        Guid monitoringObserverId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType,
        bool? isCompleted,
        DateTime lastUpdatedAt)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        PollingStationId = pollingStationId;
        MonitoringObserverId = monitoringObserverId;
        FormId = formId;
        QuestionId = questionId;
        FileName = fileName;
        FilePath = filePath;
        MimeType = mimeType;
        IsDeleted = false;

        if (isCompleted.HasValue)
        {
            IsCompleted = isCompleted.Value;
        }

        var extension = FileName.Split('.').Last();
        var uploadedFileName = $"{Id}.{extension}";
        UploadedFileName = uploadedFileName;
        LastUpdatedAt = lastUpdatedAt;
    }

    private Attachment(Guid id,
        Guid monitoringObserverId,
        Guid submissionId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType,
        bool? isCompleted,
        DateTime lastUpdatedAt)
    {
        Id = id;
        MonitoringObserverId = monitoringObserverId;
        SubmissionId = submissionId;
        QuestionId = questionId;
        FileName = fileName;
        FilePath = filePath;
        MimeType = mimeType;
        IsDeleted = false;

        if (isCompleted.HasValue)
        {
            IsCompleted = isCompleted.Value;
        }

        var extension = FileName.Split('.').Last();
        var uploadedFileName = $"{Id}.{extension}";
        UploadedFileName = uploadedFileName;
        LastUpdatedAt = lastUpdatedAt;
    }

    public void Delete()
    {
        IsDeleted = true;
    }

    public void Complete()
    {
        IsCompleted = true;
    }

    [Obsolete("Will be removed in future version")]
    public static Attachment Create(Guid id,
        Guid electionRoundId,
        Guid pollingStationId,
        Guid monitoringObserverId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType,
        DateTime lastUpdatedAt) => new(id, electionRoundId, pollingStationId, monitoringObserverId, formId, questionId,
        fileName, filePath, mimeType, false, lastUpdatedAt);

    public static Attachment CreateV2(Guid id,
        Guid submissionId,
        Guid monitoringObserverId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType,
        DateTime lastUpdatedAt) => new(id, submissionId, monitoringObserverId, questionId,
        fileName, filePath, mimeType, false, lastUpdatedAt);

#pragma warning disable CS8618 // Required by Entity Framework

    internal Attachment()
    {
    }
#pragma warning restore CS8618
}
