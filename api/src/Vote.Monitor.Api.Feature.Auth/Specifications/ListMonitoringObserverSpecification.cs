using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.Auth.Specifications;

public sealed class ListMonitoringObserverSpecification: Specification<MonitoringObserver>
{
    public ListMonitoringObserverSpecification(Guid observerId)
    {
        Query.Where(x => x.ObserverId == observerId);
    }
}
