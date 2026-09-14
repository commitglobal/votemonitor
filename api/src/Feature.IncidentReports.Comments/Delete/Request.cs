using Vote.Monitor.Core.Security;

namespace Feature.IncidentReports.Comments.Delete;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    public Guid IncidentReportId { get; set; }
    public Guid Id { get; set; }
}
