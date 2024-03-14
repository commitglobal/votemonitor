namespace Vote.Monitor.Api.Feature.Notifications.Subscribe;

public class Request
{
    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    public string Token { get; set; }
}
