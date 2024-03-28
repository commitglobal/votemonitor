using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}
