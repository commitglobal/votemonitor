namespace Vote.Monitor.Api.Feature.Emergencies.ListReceived;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim("NgoId")]
    public Guid NgoId { get; set; }
}
