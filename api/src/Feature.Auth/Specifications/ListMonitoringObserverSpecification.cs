using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Auth.Specifications;

public sealed class ListMonitoringObserverSpecification: Specification<MonitoringObserver>
{
    public ListMonitoringObserverSpecification(Guid observerId)
    {
        Query.Where(x => x.ObserverId == observerId);
    }
}
