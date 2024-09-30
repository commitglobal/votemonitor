using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;

namespace Feature.IssueReports.ListByObserver;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [QueryParam] 
    public string[]? TagsFilter { get; set; } = [];

    [QueryParam]
    public string? SearchText { get; set; }

    [QueryParam]
    public IssueReportFollowUpStatus? FollowUpStatus { get; set; }
}
