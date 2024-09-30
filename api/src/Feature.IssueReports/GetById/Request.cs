using Vote.Monitor.Core.Security;

namespace Feature.IssueReports.GetById;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid IssueReportId { get; set; }
}