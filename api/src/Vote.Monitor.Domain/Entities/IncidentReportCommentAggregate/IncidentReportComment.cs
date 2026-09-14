using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Vote.Monitor.Domain.Entities.IncidentReportCommentAggregate;

public class IncidentReportComment : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid IncidentReportId { get; private set; }
    public IncidentReport IncidentReport { get; private set; }
    public string Text { get; private set; }

    private IncidentReportComment(Guid id,
        Guid electionRoundId,
        Guid incidentReportId,
        string text)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        IncidentReportId = incidentReportId;
        Text = text;
    }

    public static IncidentReportComment Create(Guid electionRoundId,
        Guid incidentReportId,
        string text) =>
        new(Guid.NewGuid(), electionRoundId, incidentReportId, text);

    public void UpdateText(string text)
    {
        Text = text;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal IncidentReportComment()
    {
    }
#pragma warning restore CS8618
}
