using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.Citizen.Notifications.ListSent;

public class Request: BasePaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}
