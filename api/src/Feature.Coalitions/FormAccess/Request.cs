using Vote.Monitor.Core.Security;

namespace Feature.NgoCoalitions.FormAccess;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public Guid CoalitionId { get; set; }
    public Guid FormId { get; set; }
    public Guid[] NgoMembersIds { get; set; } = [];
}
