using Vote.Monitor.Core.Security;

namespace Feature.Attachments.CompleteUpload;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public Guid Id { get; set; }
    public string UploadId { get; set; }
    public Dictionary<int, string> Etags { get; set; }
}
