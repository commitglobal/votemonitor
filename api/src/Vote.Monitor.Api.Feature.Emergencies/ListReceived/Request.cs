using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Emergencies.ListReceived;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}
