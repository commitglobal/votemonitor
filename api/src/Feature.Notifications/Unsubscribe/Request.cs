using Vote.Monitor.Core.Security;

namespace Feature.Notifications.Unsubscribe;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
