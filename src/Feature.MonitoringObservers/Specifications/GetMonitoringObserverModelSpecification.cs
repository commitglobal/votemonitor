namespace Feature.MonitoringObservers.Specifications;

public sealed class GetMonitoringObserverModelSpecification : SingleResultSpecification<MonitoringObserverAggregate, MonitoringObserverModel>
{
    public GetMonitoringObserverModelSpecification(Guid electionRoundId, Guid monitoringNgoId, Guid id)
    {
        Query.Where(x => x.Id == id
                         && x.MonitoringNgo.ElectionRoundId == electionRoundId
                         && x.MonitoringNgoId == monitoringNgoId);
        
        Query.Include(x => x.Observer);
        Query.Select(entity => MonitoringObserverModel.FromEntity(entity));
    }
}
