using Vote.Monitor.Core.Security;

namespace Feature.Notes.DeleteV2;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid SubmissionId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid Id { get; set; }
}
