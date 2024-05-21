namespace Feature.MonitoringObservers.Specifications;

public sealed class GetMonitoringObserverSpecification : SingleResultSpecification<MonitoringObserverAggregate>
{
    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid ngoId, Guid id)
    {
        Query.Where(x => x.Id == id
                         && x.MonitoringNgo.ElectionRoundId == electionRoundId
                         && x.MonitoringNgo.NgoId == ngoId);
    }
}
