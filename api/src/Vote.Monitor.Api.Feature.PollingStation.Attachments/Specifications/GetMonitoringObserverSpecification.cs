using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;
public sealed class GetMonitoringObserverSpecification : SingleResultSpecification<MonitoringObserver>
{
    public GetMonitoringObserverSpecification(Guid observerId)
    {
        Query.Where(o => o.ObserverId == observerId);
    }
}
