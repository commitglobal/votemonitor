namespace Feature.IssueReports.Specifications;

public sealed class GetPollingStationSpecification : SingleResultSpecification<PollingStationAggregate>
{
    public GetPollingStationSpecification(Guid electionRoundId, Guid pollingStationId)
    {
        Query.Where(x => x.Id == pollingStationId && x.ElectionRoundId == electionRoundId);
    }
}