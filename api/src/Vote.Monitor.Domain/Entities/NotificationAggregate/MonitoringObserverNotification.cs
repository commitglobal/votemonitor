using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.NotificationAggregate;

public class MonitoringObserverNotification
{
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public Guid NotificationId { get; private set; }
    public Notification Notification { get; private set; }
    public bool IsRead { get; private set; }


    private MonitoringObserverNotification(Guid monitoringObserverId, Guid notificationId)
    {
        MonitoringObserverId = monitoringObserverId;
        NotificationId = notificationId;
    }

    public static MonitoringObserverNotification Create(Guid monitoringObserverId, Guid notificationId) =>
        new(monitoringObserverId, notificationId);

    public MonitoringObserverNotification()
    {
    }
}