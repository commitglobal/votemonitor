namespace Feature.Monitoring.Specifications;

public sealed class GetMonitoringNgoWithObserversSpecification : SingleResultSpecification<MonitoringNgoAggregate>
{
    public GetMonitoringNgoWithObserversSpecification(Guid electionRoundId, Guid monitoringNgoId)
    {
        Query
            .Where(x => x.Id == monitoringNgoId && x.ElectionRoundId == electionRoundId)
            .Include(x => x.MonitoringObservers)
            .Include(x => x.Ngo);
    }
}
