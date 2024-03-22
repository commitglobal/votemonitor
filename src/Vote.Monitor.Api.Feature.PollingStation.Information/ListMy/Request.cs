using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.ListMy;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
