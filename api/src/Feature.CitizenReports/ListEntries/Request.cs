using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.CitizenReports.ListEntries;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [QueryParam] public string? SearchText { get; set; }

    [QueryParam] public bool? HasFlaggedAnswers { get; set; }

    [QueryParam] public CitizenReportFollowUpStatus? FollowUpStatus { get; set; }
}