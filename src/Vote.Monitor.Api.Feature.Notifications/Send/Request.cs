namespace Vote.Monitor.Api.Feature.Notifications.Send;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim("NgoId")]
    public Guid NgoId { get; set; }

    public List<Guid> ObserverIds { get; set; } = new();

    public string Title { get; set; }
    public string Body { get; set; }
}
