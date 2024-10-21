using Vote.Monitor.Domain.Entities.NgoAdminAggregate;

namespace Vote.Monitor.Domain.Entities.CitizenNotificationAggregate;

public class CitizenNotification : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid SenderId { get; private set; }
    public NgoAdmin Sender { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }

    private CitizenNotification(Guid electionRoundId,
        Guid senderId,
        string title,
        string body)
    {
        Id = Guid.NewGuid();
        ElectionRoundId = electionRoundId;
        SenderId = senderId;

        Title = title;
        Body = body;
    }

    public static CitizenNotification Create(Guid electionRoundId,
        Guid senderId,
        string title,
        string body) => new(electionRoundId, senderId, title, body);

#pragma warning disable CS8618 // Required by Entity Framework

    internal CitizenNotification()
    {
    }
#pragma warning restore CS8618
}