namespace Vote.Monitor.Api.Feature.PollingStation.Information.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}
