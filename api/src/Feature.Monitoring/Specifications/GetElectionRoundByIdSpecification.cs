namespace Feature.Monitoring.Specifications;

public sealed class GetElectionRoundByIdSpecification: SingleResultSpecification<ElectionRoundAggregate>
{
    public GetElectionRoundByIdSpecification(Guid electionRoundId)
    {
        Query
            .Where(x => x.Id == electionRoundId)
            .Include(x => x.MonitoringNgos);
    }
}
