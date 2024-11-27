using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notifications.Specifications;

public sealed class GetMonitoringObserverSpecification : Specification<MonitoringObserver>
{
    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid ngoId, List<Guid> monitoringObserverIds)
    {
        Query
            .Where(x =>
                x.ElectionRoundId == electionRoundId
                && x.MonitoringNgo.NgoId == ngoId
                && x.MonitoringNgo.ElectionRoundId == electionRoundId
                && monitoringObserverIds.Contains(x.Id));
    }
}
