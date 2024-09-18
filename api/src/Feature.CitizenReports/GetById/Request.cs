using Vote.Monitor.Core.Security;

namespace Feature.CitizenReports.GetById;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid CitizenReportId { get; set; }
}