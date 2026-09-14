using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Vote.Monitor.Domain.Entities.QuickReportCommentAggregate;

public class QuickReportComment : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid QuickReportId { get; private set; }
    public QuickReport QuickReport { get; private set; }
    public string Text { get; private set; }

    private QuickReportComment(Guid id,
        Guid electionRoundId,
        Guid quickReportId,
        string text)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        QuickReportId = quickReportId;
        Text = text;
    }

    public static QuickReportComment Create(Guid electionRoundId,
        Guid quickReportId,
        string text) =>
        new(Guid.NewGuid(), electionRoundId, quickReportId, text);

    public void UpdateText(string text)
    {
        Text = text;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal QuickReportComment()
    {
    }
#pragma warning restore CS8618
}
