using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Domain.Entities.IssueReportNoteAggregate;

public class IssueReportNote : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public Guid FormId { get; private set; }
    public Guid IssueReportId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; }

    public ElectionRound ElectionRound { get; private set; }
    public Form Form { get; private set; }
    public IssueReport IssueReport { get; private set; }

    public IssueReportNote(Guid id,
        Guid electionRoundId,
        Guid issueReportId,
        Guid formId,
        Guid questionId,
        string text) : base(id)
    {
        ElectionRoundId = electionRoundId;
        FormId = formId;
        IssueReportId = issueReportId;
        QuestionId = questionId;
        Text = text;
    }

    public void UpdateText(string text)
    {
        Text = text;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    internal IssueReportNote()
    {
    }
#pragma warning restore CS8618
}