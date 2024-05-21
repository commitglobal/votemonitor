namespace Feature.Notifications.ListRecipients;

public record Response
{
    public required string? NgoName { get; set; }
    public required List<ReceivedNotificationModel> Notifications { get; init; }
}

public record ReceivedNotificationModel
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required string Sender { get; init; }
    public required DateTime SentAt { get; init; }
    public required string NgoName { get; set; }
}
