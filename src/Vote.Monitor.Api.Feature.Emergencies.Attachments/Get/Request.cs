namespace Vote.Monitor.Api.Feature.Emergencies.Attachments.Get;

public class Request
{
    public  Guid ElectionRoundId { get; set; }

    public  Guid PollingStationId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    public  Guid Id { get; set; }
}
