namespace Vote.Monitor.Api.Feature.Emergencies.Create;

public class Request
{
    public required Guid ElectionRoundId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }
}
