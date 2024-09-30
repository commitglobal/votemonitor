using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Domain.Entities.IssueReportAttachmentAggregate;

public class IssueReportAttachment : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }

    public Guid IssueReportId { get; private set; }
    public IssueReport IssueReport { get; private set; }

    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public Guid QuestionId { get; private set; }
    public string FileName { get; private set; }
    public string UploadedFileName { get; private set; }
    public string FilePath { get; private set; }
    public string MimeType { get; private set; }

    public bool IsDeleted { get; private set; }
    public bool IsCompleted { get; private set; }

    private IssueReportAttachment(Guid id,
        Guid electionRoundId,
        Guid issueReportId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType,
        bool isCompleted) : base(id)
    {
        ElectionRoundId = electionRoundId;
        IssueReportId = issueReportId;
        FormId = formId;
        QuestionId = questionId;
        FileName = fileName;
        FilePath = filePath;
        MimeType = mimeType;
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

    public static IssueReportAttachment Create(Guid id,
        Guid electionRoundId,
        Guid issueReportId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, issueReportId, formId, questionId, fileName, filePath, mimeType,
        true);

    public static IssueReportAttachment CreateV2(Guid id,
        Guid electionRoundId,
        Guid issueReportId,
        Guid formId,
        Guid questionId,
        string fileName,
        string filePath,
        string mimeType) => new(id, electionRoundId, issueReportId, formId, questionId, fileName, filePath, mimeType,
        false);

#pragma warning disable CS8618 // Required by Entity Framework

    internal IssueReportAttachment()
    {
    }
#pragma warning restore CS8618
}