using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Vote.Monitor.Domain.Entities.IncidentReportAttachmentAggregate;

public class IncidentReportAttachment : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }

    public Guid IncidentReportId { get; private set; }
    public IncidentReport IncidentReport { get; private set; }

    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public Guid QuestionId { get; private set; }
    public string FileName { get; private set; }
    public string UploadedFileName { get; private set; }
    public string FilePath { get; private set; }
    public string MimeType { get; private set; }

    public bool IsDeleted { get; private set; }
    public bool IsCompleted { get; private set; }

    private IncidentReportAttachment(Guid id,
        Guid electionRoundId,
        Guid incidentReportId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType,
        bool? isCompleted) : base(id)
    {
        ElectionRoundId = electionRoundId;
        IncidentReportId = incidentReportId;
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

    public static IncidentReportAttachment Create(Guid id,
        Guid electionRoundId,
        Guid incidentReportId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, incidentReportId, formId, questionId, fileName, filePath, mimeType,
        true);

    public static IncidentReportAttachment CreateV2(Guid id,
        Guid electionRoundId,
        Guid incidentReportId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, incidentReportId, formId, questionId, fileName, filePath, mimeType,
        false);

#pragma warning disable CS8618 // Required by Entity Framework

    internal IncidentReportAttachment()
    {
    }
#pragma warning restore CS8618
}