using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;

public sealed class GetMonitoringObserverSpecification: SingleResultSpecification<MonitoringObserver>
{
    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid observerId)
    {
        Query.Where(x => x.ObserverId == observerId && x.InviterNgo.ElectionRoundId == electionRoundId);
    }
}
