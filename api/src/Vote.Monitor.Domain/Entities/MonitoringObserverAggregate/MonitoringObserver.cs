using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ObserverAggregate;

namespace Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

public class MonitoringObserver : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public MonitoringNgo MonitoringNgo { get; private set; }

    public Guid ObserverId { get; private set; }
    public Observer Observer { get; private set; }
    public string? PhoneNumber { get; private set; }

    public MonitoringObserverStatus Status { get; private set; }

    public string[] Tags { get; private set; }

    private MonitoringObserver(Guid electionRoundId, Guid monitoringNgoId, Guid observerId, string[] tags,
        MonitoringObserverStatus status, string? phoneNumber = null)
    {
        Id = Guid.NewGuid();
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
        ObserverId = observerId;
        Tags = tags;
        Status = status;
        PhoneNumber = phoneNumber;
    }

    public void Activate()
    {
        Status = MonitoringObserverStatus.Active;
    }

    public void Suspend()
    {
        Status = MonitoringObserverStatus.Suspended;
    }

    public static MonitoringObserver Create(Guid electionRoundId, Guid monitoringNgoId, Guid observerId, string[] tags)
    {
        return new MonitoringObserver(electionRoundId, monitoringNgoId, observerId, tags,
            MonitoringObserverStatus.Pending);
    }

    public static MonitoringObserver CreateForExisting(Guid electionRoundId,
        Guid monitoringNgoId,
        Guid observerId,
        string[] tags,
        UserStatus accountStatus)
    {
        MonitoringObserverStatus status = accountStatus == UserStatus.Active
            ? MonitoringObserverStatus.Active
            : accountStatus == UserStatus.Pending
                ? MonitoringObserverStatus.Pending
                : MonitoringObserverStatus.Suspended;

        return new MonitoringObserver(electionRoundId, monitoringNgoId, observerId, tags, status);
    }

    public void Update(MonitoringObserverStatus status, string[] tags)
    {
        if (status != MonitoringObserverStatus.Pending)
        {
            Status = status;
        }

        Tags = tags;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private MonitoringObserver()
    {
    }
#pragma warning restore CS8618
}
