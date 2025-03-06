using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notes.Specifications;

public sealed class GetMonitoringObserverIdSpecification : SingleResultSpecification<MonitoringObserver, Guid>
{
    public GetMonitoringObserverIdSpecification(Guid electionRoundId, Guid observerId)
    {
        Query.Where(o =>
            o.ElectionRoundId == electionRoundId && o.ObserverId == observerId &&
            o.MonitoringNgo.ElectionRoundId == electionRoundId);

        Query.Select(x => x.Id);
    }
}
