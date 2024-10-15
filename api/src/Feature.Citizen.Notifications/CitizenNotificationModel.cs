using Vote.Monitor.Domain.Entities.CitizenNotificationAggregate;

namespace Feature.Citizen.Notifications;

public record CitizenNotificationModel
{
    public required Guid Id { get; set; }
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required string Sender { get; init; }
    public required DateTime SentAt { get; init; }

    public static CitizenNotificationModel FromEntity(CitizenNotification entity) =>
        new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Body = entity.Body,
            Sender = entity.SenderId.ToString(),
            SentAt = entity.CreatedOn
        };
}