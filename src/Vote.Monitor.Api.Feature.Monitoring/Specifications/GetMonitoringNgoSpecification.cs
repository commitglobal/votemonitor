namespace Vote.Monitor.Api.Feature.Monitoring.Specifications;

public sealed class GetMonitoringNgoSpecification : SingleResultSpecification<MonitoringNgoAggregate>
{
    public GetMonitoringNgoSpecification(Guid electionRoundId, Guid monitoringNgoId)
    {
        Query
            .Where(x => x.Id == monitoringNgoId && x.ElectionRoundId == electionRoundId)
            .Include(x => x.MonitoringObservers)
            .Include(x => x.Ngo);
    }
}
