using Vote.Monitor.Core.Security;

namespace Feature.CitizenReports.Comments.Delete;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    public Guid CitizenReportId { get; set; }
    public Guid Id { get; set; }
}
