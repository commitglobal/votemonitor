using Vote.Monitor.Core.Security;

namespace Feature.Attachments.ListV2;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid SubmissionId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

}
