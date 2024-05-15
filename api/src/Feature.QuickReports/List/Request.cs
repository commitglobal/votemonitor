using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.QuickReports.List;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}
