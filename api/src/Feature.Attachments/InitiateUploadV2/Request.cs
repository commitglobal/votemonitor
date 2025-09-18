using Vote.Monitor.Core.Security;

namespace Feature.Attachments.InitiateUploadV2;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid SubmissionId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public int NumberOfUploadParts { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
