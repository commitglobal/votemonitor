using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.ListMy;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    [QueryParam] 
    public List<Guid>? PollingStationIds { get; set; }
}
