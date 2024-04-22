using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notes.Specifications;
public sealed class GetMonitoringObserverSpecification : Specification<MonitoringObserver>
{
    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid observerId)
    {
        Query.Where(o => o.ElectionRoundId == electionRoundId && o.ObserverId == observerId);
    }
}
