namespace Vote.Monitor.Api.Feature.PollingStation.Information.ListMy;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }
}
