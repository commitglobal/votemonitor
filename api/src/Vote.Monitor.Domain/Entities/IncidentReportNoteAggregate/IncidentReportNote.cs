using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Vote.Monitor.Domain.Entities.IncidentReportNoteAggregate;

public class IncidentReportNote : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public Guid FormId { get; private set; }
    public Guid IncidentReportId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; }

    public ElectionRound ElectionRound { get; private set; }
    public Form Form { get; private set; }
    public IncidentReport IncidentReport { get; private set; }

    public IncidentReportNote(Guid id,
        Guid electionRoundId,
        Guid incidentReportId,
        Guid formId,
        Guid questionId,
        string text) : base(id)
    {
        ElectionRoundId = electionRoundId;
        FormId = formId;
        IncidentReportId = incidentReportId;
        QuestionId = questionId;
        Text = text;
    }

    public void UpdateText(string text)
    {
        Text = text;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    internal IncidentReportNote()
    {
    }
#pragma warning restore CS8618
}