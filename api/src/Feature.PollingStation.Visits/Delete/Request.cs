using Vote.Monitor.Core.Security;

namespace Feature.PollingStation.Visits.Delete;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid PollingStationId { get; set; }
}
