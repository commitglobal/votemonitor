namespace Feature.PollingStations.Specifications;

public sealed class GetPollingStationsByIdsInOtherElectionRoundsSpecification : Specification<PollingStationAggregate>
{
    public GetPollingStationsByIdsInOtherElectionRoundsSpecification(Guid electionRoundId, IEnumerable<Guid> pollingStationIds)
    {
        Query
            .Where(x => x.ElectionRoundId != electionRoundId)
            .Where(x => pollingStationIds.Contains(x.Id));
    }
}
