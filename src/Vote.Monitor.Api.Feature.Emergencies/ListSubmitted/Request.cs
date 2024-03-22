using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Emergencies.ListSubmitted;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
