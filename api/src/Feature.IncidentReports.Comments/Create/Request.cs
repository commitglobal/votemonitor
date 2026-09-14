using Vote.Monitor.Core.Security;

namespace Feature.IncidentReports.Comments.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid IncidentReportId { get; set; }
    public string Text { get; set; } = string.Empty;
}
