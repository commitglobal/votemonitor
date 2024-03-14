namespace Vote.Monitor.Api.Feature.Emergencies.Delete;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    public Guid Id { get; set; }
}
