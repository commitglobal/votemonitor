using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.Any;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
