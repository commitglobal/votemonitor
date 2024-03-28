using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Delete;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
