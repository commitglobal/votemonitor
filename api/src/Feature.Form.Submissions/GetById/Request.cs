using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.GetById;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public Guid SubmissionId { get; set; }
}
