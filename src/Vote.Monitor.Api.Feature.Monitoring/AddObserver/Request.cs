namespace Vote.Monitor.Api.Feature.Monitoring.AddObserver;

public class Request
{
    public Guid Id { get; set; }
    public Guid ObserverId { get; set; }
    public Guid InviterNgoId { get; set; }
}
