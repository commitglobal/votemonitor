namespace Vote.Monitor.Api.Feature.Monitoring.RemoveObserver;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid NgoId { get; set; }
    public Guid ObserverId { get; set; }
}
