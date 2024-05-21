namespace Feature.Notifications;

public record NotificationModel
{
    public required Guid Id { get; set; }
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required string Sender { get; init; }
    public required DateTime SentAt { get; init; }
    public required int NumberOfTargetedObservers { get; init; }
}
