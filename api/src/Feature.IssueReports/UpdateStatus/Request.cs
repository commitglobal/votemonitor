using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;

namespace Feature.IssueReports.UpdateStatus;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid Id { get; set; }
    public IssueReportFollowUpStatus FollowUpStatus { get; set; }
}