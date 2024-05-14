namespace Feature.Notifications.ListReceived;

public record Response
{
    public required List<ReceivedNotificationModel> Notifications { get; init; }
}

public record ReceivedNotificationModel
{
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required string Sender { get; init; }
    public required DateTime SentAt { get; init; }
}
