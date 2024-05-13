using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.UpdateNeedsFollowUpStatus;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public Guid Id { get; set; }
    public bool NeedsFollowUp { get; set; }
}
