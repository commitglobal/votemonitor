namespace Feature.Notifications;

public record NotificationDetailedModel
{
    public required Guid Id { get; set; }
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required string Sender { get; init; }
    public required DateTime SentAt { get; init; }
    public required NotificationReceiver[] Receivers { get; init; } = [];
}

public record NotificationReceiver
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }
}
