namespace Vote.Monitor.Domain.Entities.NotificationTokenAggregate;

public class NotificationToken : BaseEntity, IAggregateRoot
{
    public Guid ObserverId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Token { get; private set; }

    public static NotificationToken Create(Guid observerId, string token, ITimeProvider timeProvider)
        => new(observerId, timeProvider.UtcNow, token, timeProvider);

    private NotificationToken(Guid observerId, DateTime timestamp, string token, ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        ObserverId = observerId;
        Timestamp = timestamp;
        Token = token;
    }

    public void Update(string token, ITimeProvider timeProvider)
    {
        Token = token;
        Timestamp = timeProvider.UtcNow;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal NotificationToken()
    {

    }
#pragma warning restore CS8618
}
