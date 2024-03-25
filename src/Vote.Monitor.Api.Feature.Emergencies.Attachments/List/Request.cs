using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Emergencies.Attachments.List;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
