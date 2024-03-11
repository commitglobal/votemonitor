namespace Vote.Monitor.Api.Feature.Monitoring.AddNgo;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid NgoId { get; set; }
}
