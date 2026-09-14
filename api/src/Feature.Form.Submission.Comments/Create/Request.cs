using Vote.Monitor.Core.Security;

namespace Feature.Form.Submission.Comments.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid? QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
}
