using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.AddObserver;
public record class MonitoringObserverModel
{
    public required Guid Id { get; init; }
    public Guid InviterNgoId { get; init; }
    public MonitoringObserverStatus Status { get; init; }
}
