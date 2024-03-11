namespace Vote.Monitor.Api.Feature.PushNotifications.ListReceived;

public class Request
{
    public required Guid ElectionRoundId { get; set; }

    public required Guid PollingStationId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    public required IFormFile Attachment { get; set; }
}
