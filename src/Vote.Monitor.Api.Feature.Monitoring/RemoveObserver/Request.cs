namespace Vote.Monitor.Api.Feature.Monitoring.RemoveObserver;

public class Request
{
    public Guid Id { get; set; }
    public Guid ObserverId { get; set; }
}
