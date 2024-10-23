namespace Feature.Notifications.Send;

public class NotificationRecipient
{
    public Guid MonitoringObserverId { get; set; }
    public string? Token { get; set; }
}
