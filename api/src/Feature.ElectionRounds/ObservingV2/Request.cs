using Vote.Monitor.Core.Security;

namespace Feature.ElectionRounds.ObservingV2;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }
}
