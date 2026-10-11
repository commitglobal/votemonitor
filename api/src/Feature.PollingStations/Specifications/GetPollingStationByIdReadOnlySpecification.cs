namespace Feature.PollingStations.Specifications;

public sealed class GetPollingStationByIdReadOnlySpecification : Specification<PollingStationAggregate>,
    ISingleResultSpecification<PollingStationAggregate>
{
    public GetPollingStationByIdReadOnlySpecification(Guid electionRoundId, Guid id)
    {
        Query.AsNoTracking();

        Query.Where(pollingStation => pollingStation.ElectionRoundId == electionRoundId && pollingStation.Id == id);
    }
}
