using Vote.Monitor.Core.Security;

namespace Feature.QuickReports.Comments.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid QuickReportId { get; set; }
    public string Text { get; set; } = string.Empty;
}
