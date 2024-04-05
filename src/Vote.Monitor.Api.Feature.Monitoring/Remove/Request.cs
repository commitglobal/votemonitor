namespace Vote.Monitor.Api.Feature.Monitoring.Remove;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid NgoId { get; set; }
}
