using Vote.Monitor.Core.Security;

namespace Feature.MonitoringObservers.ClearTags;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public Guid[] MonitoringObserverIds { get; set; } = [];
}
