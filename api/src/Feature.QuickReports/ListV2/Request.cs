using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.QuickReports.ListV2;

public class Request : BaseFilterConditionsSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public DataSource DataSource { get; set; } = DataSource.Ngo;
    [QueryParam] public Guid? CoalitionMemberId { get; set; }
}
