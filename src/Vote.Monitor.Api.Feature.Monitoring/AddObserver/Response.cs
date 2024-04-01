namespace Vote.Monitor.Api.Feature.Monitoring.AddObserver;
public record Response
{
    public Guid Id { get; init; }
    public Guid InviterNgoId { get; init; }
    public Guid ObserverId { get; init; }
}
