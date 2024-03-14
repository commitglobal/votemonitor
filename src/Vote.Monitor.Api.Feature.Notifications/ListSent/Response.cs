namespace Vote.Monitor.Api.Feature.Notifications.ListSent;

public record Response
{
    public required List<SentNotificationModel> Notifications { get; init; }
}
public record SentNotificationModel
{
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required string Sender { get; init; }
    public required DateTime SentAt { get; init; }
    public required List<NotificationReceiver> Receivers { get; init; } = new();
}

public record NotificationReceiver
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }
}
