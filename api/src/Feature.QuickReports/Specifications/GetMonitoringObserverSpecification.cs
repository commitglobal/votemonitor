using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.QuickReports.Specifications;
public sealed class GetMonitoringObserverSpecification : SingleResultSpecification<MonitoringObserver>
{
    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid observerId)
    {
        Query.Where(o => o.ElectionRoundId == electionRoundId && o.ObserverId == observerId);
    }
}
