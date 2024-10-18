using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAdminAggregate;

namespace Vote.Monitor.Domain.Entities.NotificationAggregate;

public class Notification : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid SenderId { get; private set; }
    public NgoAdmin Sender { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public IReadOnlyList<MonitoringObserver> TargetedObservers { get; private set; }

    private Notification(Guid electionRoundId,
        Guid senderId,
        IEnumerable<MonitoringObserver> observers,
        string title,
        string body)
    {
        Id = Guid.NewGuid();
        ElectionRoundId = electionRoundId;
        SenderId = senderId;

        TargetedObservers = observers.ToList().AsReadOnly();
        Title = title;
        Body = body;
    }

    public static Notification Create(Guid electionRoundId,
        Guid senderId,
        IEnumerable<MonitoringObserver> observers,
        string title,
        string body) => new(electionRoundId, senderId, observers, title, body);

#pragma warning disable CS8618 // Required by Entity Framework

    internal Notification()
    {
    }
#pragma warning restore CS8618
}
