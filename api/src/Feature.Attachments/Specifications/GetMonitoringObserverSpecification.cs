using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Attachments.Specifications;

public sealed class GetMonitoringObserverIdSpecification : SingleResultSpecification<MonitoringObserver, Guid>
{
    public GetMonitoringObserverIdSpecification(Guid electionRoundId, Guid observerId)
    {
        Query.Where(o => o.ElectionRoundId == electionRoundId && o.ObserverId == observerId);
        Query.Select(x => x.Id);
    }
}
