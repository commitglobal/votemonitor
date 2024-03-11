namespace Vote.Monitor.Api.Feature.PushNotifications.Subscribe;

public class Request
{
    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    public string Token { get; set; }
}
