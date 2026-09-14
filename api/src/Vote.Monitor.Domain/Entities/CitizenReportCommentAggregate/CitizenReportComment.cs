using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Vote.Monitor.Domain.Entities.CitizenReportCommentAggregate;

public class CitizenReportComment : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid CitizenReportId { get; private set; }
    public CitizenReport CitizenReport { get; private set; }
    public string Text { get; private set; }

    private CitizenReportComment(Guid id,
        Guid electionRoundId,
        Guid citizenReportId,
        string text)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        CitizenReportId = citizenReportId;
        Text = text;
    }

    public static CitizenReportComment Create(Guid electionRoundId,
        Guid citizenReportId,
        string text) =>
        new(Guid.NewGuid(), electionRoundId, citizenReportId, text);

    public void UpdateText(string text)
    {
        Text = text;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal CitizenReportComment()
    {
    }
#pragma warning restore CS8618
}
