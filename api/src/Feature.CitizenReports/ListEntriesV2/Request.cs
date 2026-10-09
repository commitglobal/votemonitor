using Vote.Monitor.Core.Security;

namespace Feature.CitizenReports.ListEntriesV2;

public class Request : BaseFilterConditionsSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}
