namespace Feature.Monitoring.Specifications;

public sealed class GetMonitoringNgoSpecification : SingleResultSpecification<MonitoringNgoAggregate>
{
    public GetMonitoringNgoSpecification(Guid electionRoundId, Guid ngoId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.NgoId == ngoId);
    }
}
