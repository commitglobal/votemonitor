using Vote.Monitor.Core.Security;

namespace Feature.IncidentReports.ListFormsOverviewV2;

public class Request : BaseFilterConditionsSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}
