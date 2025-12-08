using Vote.Monitor.Core.Security;

namespace Feature.Notes.ListV2;

public class Request
{
    public Guid ElectionRoundId { get; set; }


    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid SubmissionId { get; set; }
}
