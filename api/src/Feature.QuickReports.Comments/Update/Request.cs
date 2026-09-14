using Vote.Monitor.Core.Security;

namespace Feature.QuickReports.Comments.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    public Guid QuickReportId { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}
