using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Notifications.Subscribe;

public class Request
{
    [FromClaim(ClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public string Token { get; set; }
}
