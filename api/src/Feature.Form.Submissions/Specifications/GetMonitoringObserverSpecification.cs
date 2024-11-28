using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Form.Submissions.Specifications;

public sealed class GetMonitoringObserverSpecification: SingleResultSpecification<MonitoringObserver>
{
    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid observerId)
    {
        Query.Where(x => x.ObserverId == observerId && x.MonitoringNgo.ElectionRoundId == electionRoundId && x.ElectionRoundId == electionRoundId);
    }
}
