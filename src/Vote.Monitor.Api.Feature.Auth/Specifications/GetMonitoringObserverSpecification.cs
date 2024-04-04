using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.Auth.Specifications;

public sealed class GetMonitoringObserverSpecification : SingleResultSpecification<MonitoringObserver>
{
    public GetMonitoringObserverSpecification(Guid observerId)
    {
        Query.Include(x=> x.InviterNgo)
            .Where(x => x.ObserverId == observerId);
    }
}
