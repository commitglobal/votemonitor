using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Domain.Entities.CitizenReportAttachmentAggregate;

public class CitizenReportAttachment : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }

    public Guid CitizenReportId { get; private set; }
    public CitizenReport CitizenReport { get; private set; }

    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public Guid QuestionId { get; private set; }
    public string FileName { get; private set; }
    public string UploadedFileName { get; private set; }
    public string FilePath { get; private set; }
    public string MimeType { get; private set; }

    public bool IsDeleted { get; private set; }
    public bool IsCompleted { get; private set; }

    private CitizenReportAttachment(Guid id,
        Guid electionRoundId,
        Guid citizenReportId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType,
        bool? isCompleted)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        CitizenReportId = citizenReportId;
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

    public static CitizenReportAttachment Create(Guid id,
        Guid electionRoundId,
        Guid citizenReportId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, citizenReportId, formId, questionId, fileName, filePath, mimeType,
        true);

    public static CitizenReportAttachment CreateV2(Guid id,
        Guid electionRoundId,
        Guid citizenReportId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, citizenReportId, formId, questionId, fileName, filePath, mimeType,
        false);

#pragma warning disable CS8618 // Required by Entity Framework

    internal CitizenReportAttachment()
    {
    }
#pragma warning restore CS8618
}