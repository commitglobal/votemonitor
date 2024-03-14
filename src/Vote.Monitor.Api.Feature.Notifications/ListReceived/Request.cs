namespace Vote.Monitor.Api.Feature.Notifications.ListReceived;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }
}
