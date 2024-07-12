using Vote.Monitor.Core.Security;

namespace Feature.Forms.Duplicate;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public Guid Id { get; set; }
}
