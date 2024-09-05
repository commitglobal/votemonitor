using Vote.Monitor.Core.Security;

namespace Feature.CitizenReports.ListFormsOverview;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}
