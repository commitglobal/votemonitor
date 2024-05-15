namespace Feature.Notifications.ListSent;

public record Response
{
    public required List<NotificationModel> Notifications { get; init; }
}
