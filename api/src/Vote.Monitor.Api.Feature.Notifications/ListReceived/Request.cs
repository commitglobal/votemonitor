using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Notifications.ListReceived;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
