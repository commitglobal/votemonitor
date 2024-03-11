namespace Vote.Monitor.Api.Feature.Monitoring.Specifications;

public sealed class GetMonitoringNgoSpecification : SingleResultSpecification<MonitoringNgoAggregate>
{
    public GetMonitoringNgoSpecification(Guid electionRoundId, Guid ngoId)
    {
        Query
            .Where(x => x.NgoId == ngoId && x.ElectionRoundId == electionRoundId)
            .Include(x => x.MonitoringObservers);
    }
}
