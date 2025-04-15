using Vote.Monitor.Core.Security;

namespace Feature.ElectionRounds.Monitoring;

public class Request
{
    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}
