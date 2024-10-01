using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Feature.CitizenReports.UpdateStatus;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid Id { get; set; }
    public CitizenReportFollowUpStatus FollowUpStatus { get; set; }
}