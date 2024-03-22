using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Emergencies.Create;

public class Request
{
    public required Guid ElectionRoundId { get; set; }

    [FromClaim(ClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
