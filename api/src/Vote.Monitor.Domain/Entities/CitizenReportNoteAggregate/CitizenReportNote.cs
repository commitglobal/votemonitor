using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Domain.Entities.CitizenReportNoteAggregate;

public class CitizenReportNote : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public Guid FormId { get; private set; }
    public Guid CitizenReportId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; }

    public ElectionRound ElectionRound { get; private set; }
    public Form Form { get; private set; }
    public CitizenReport CitizenReport { get; private set; }

    public CitizenReportNote(Guid id,
        Guid electionRoundId,
        Guid citizenReportId,
        Guid formId,
        Guid questionId,
        string text) : base(id)
    {
        ElectionRoundId = electionRoundId;
        FormId = formId;
        CitizenReportId = citizenReportId;
        QuestionId = questionId;
        Text = text;
    }

    public void UpdateText(string text)
    {
        Text = text;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    internal CitizenReportNote()
    {
    }
#pragma warning restore CS8618
}