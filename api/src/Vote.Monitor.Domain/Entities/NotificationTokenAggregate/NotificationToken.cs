namespace Vote.Monitor.Domain.Entities.NotificationTokenAggregate;

public class NotificationToken : AuditableBaseEntity, IAggregateRoot
{
    public Guid ObserverId { get; private set; }
    public string Token { get; private set; }

    public static NotificationToken Create(Guid observerId, string token)
        => new(observerId, token);

    private NotificationToken(Guid observerId, string token) : base(Guid.NewGuid())
    {
        ObserverId = observerId;
        Token = token;
    }

    public void Update(string token)
    {
        Token = token;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal NotificationToken()
    {

    }
#pragma warning restore CS8618
}
