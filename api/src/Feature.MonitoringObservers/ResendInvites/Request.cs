using Vote.Monitor.Core.Security;

namespace Feature.MonitoringObservers.ResendInvites;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public List<Guid> Ids { get; set; }
}
