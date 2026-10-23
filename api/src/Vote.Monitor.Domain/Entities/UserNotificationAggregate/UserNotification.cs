using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.UserNotificationAggregate;

/// <summary>
/// An NGO admin's or staff member's inbox item: a review change that concerns them.
/// </summary>
public class UserNotification : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid ActivityId { get; private set; }
    public ReviewActivity Activity { get; private set; }
    public ReportRecordType RecordType { get; private set; }
    public Guid RecordId { get; private set; }
    public UserNotificationReason Reason { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public static UserNotification Create(Guid userId, ReviewActivity activity, UserNotificationReason reason) =>
        new(Guid.NewGuid(), userId, activity, reason);

    public void MarkAsRead(DateTime now) => ReadAt ??= now;

    private UserNotification(Guid id, Guid userId, ReviewActivity activity, UserNotificationReason reason)
    {
        Id = id;
        UserId = userId;
        ElectionRoundId = activity.ElectionRoundId;
        ActivityId = activity.Id;
        Activity = activity;
        RecordType = activity.RecordType;
        RecordId = activity.RecordId;
        Reason = reason;
        CreatedOn = activity.OccurredAt;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private UserNotification()
    {
    }
#pragma warning restore CS8618
}
