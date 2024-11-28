using Vote.Monitor.Core.Security;

namespace Feature.NgoCoalitions.GetMy;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    
    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}
