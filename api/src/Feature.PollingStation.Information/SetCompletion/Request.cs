using Vote.Monitor.Core.Security;

namespace Feature.PollingStation.Information.SetCompletion;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public bool IsCompleted { get; set; }
}
