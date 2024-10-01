using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.ListByObserver;

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
    public IncidentReportFollowUpStatus? FollowUpStatus { get; set; }
}
