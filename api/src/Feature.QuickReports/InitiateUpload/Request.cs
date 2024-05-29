using Vote.Monitor.Core.Security;

namespace Feature.QuickReports.InitiateUpload;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public Guid QuickReportId { get; set; }
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public int NumberOfUploadParts { get; set; }
}
