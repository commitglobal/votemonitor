using Vote.Monitor.Core.Security;

namespace Feature.IncidentReports.GetSubmissionsAggregatedV2;

public class Request : BaseFilterConditionsRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid FormId { get; set; }
}
