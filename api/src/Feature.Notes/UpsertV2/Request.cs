using Vote.Monitor.Core.Security;

namespace Feature.Notes.UpsertV2;

public class Request
{
    public Guid ElectionRoundId { get; set; }


    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime LastUpdatedAt { get; set; }
}
