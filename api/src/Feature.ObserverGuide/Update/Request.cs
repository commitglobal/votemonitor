using Vote.Monitor.Core.Security;

namespace Feature.ObserverGuide.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; }
}
