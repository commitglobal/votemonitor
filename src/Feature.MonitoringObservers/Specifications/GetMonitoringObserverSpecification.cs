namespace Feature.MonitoringObservers.Specifications;

public sealed class GetMonitoringObserverSpecification : SingleResultSpecification<MonitoringObserverAggregate>
{
    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid monitoringNgoId, Guid id)
    {
        Query.Where(x => x.Id == id
                         && x.MonitoringNgo.ElectionRoundId == electionRoundId
                         && x.MonitoringNgoId == monitoringNgoId);
    }
}
