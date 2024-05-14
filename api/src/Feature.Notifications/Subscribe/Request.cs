using Vote.Monitor.Core.Security;

namespace Feature.Notifications.Subscribe;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public string Token { get; set; }
}
