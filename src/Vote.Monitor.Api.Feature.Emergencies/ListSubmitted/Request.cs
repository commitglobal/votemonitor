namespace Vote.Monitor.Api.Feature.Emergencies.ListSubmitted;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }
}
