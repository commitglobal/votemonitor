using Vote.Monitor.Core.Security;

namespace Feature.Forms.GetFormsVersion;

public class Request
{
    public required Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public required Guid ObserverId { get; set; }
}
