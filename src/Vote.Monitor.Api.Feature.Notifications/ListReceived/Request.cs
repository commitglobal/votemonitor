using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Notifications.ListReceived;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
