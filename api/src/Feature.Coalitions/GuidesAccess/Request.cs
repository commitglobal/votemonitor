using Vote.Monitor.Core.Security;

namespace Feature.NgoCoalitions.GuidesAccess;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public Guid CoalitionId { get; set; }
    public Guid GuideId { get; set; }
    public Guid[] NgoMembersIds { get; set; } = [];
}
