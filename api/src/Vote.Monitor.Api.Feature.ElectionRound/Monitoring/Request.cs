using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.ElectionRound.Monitoring;

public class Request
{
    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}
