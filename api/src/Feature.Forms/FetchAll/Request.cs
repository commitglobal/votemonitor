using Vote.Monitor.Core.Security;

namespace Feature.Forms.FetchAll;
public class Request
{
    public Guid ElectionRoundId { get; set; }
    
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
