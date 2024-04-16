using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.Notifications.Specifications;

public sealed class GetMonitoringObserverSpecification : Specification<MonitoringObserver>
{
    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid ngoId, List<Guid> monitoringObserverIds)
    {
        Query
            .Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.MonitoringNgo.NgoId == ngoId
            && monitoringObserverIds.Contains(x.Id));
    }
}
