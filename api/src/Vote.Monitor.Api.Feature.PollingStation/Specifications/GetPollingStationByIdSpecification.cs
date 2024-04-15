namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public sealed class GetPollingStationByIdSpecification : Specification<PollingStationAggregate>,
    ISingleResultSpecification<PollingStationAggregate>
{
    public GetPollingStationByIdSpecification(Guid electionRoundId, Guid id)
    {
        Query.Where(pollingStation => pollingStation.ElectionRoundId == electionRoundId && pollingStation.Id == id);
    }
}
