namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public sealed class GetPollingStationSpecification : Specification<PollingStationAggregate>
{
    public GetPollingStationSpecification(Guid electionRoundId,
        string level1,
        string level2,
        string level3,
        string level4,
        string level5,
        string number,
        string address)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => x.Level1 == level1)
            .Where(x => x.Level2 == level2)
            .Where(x => x.Level3 == level3)
            .Where(x => x.Level4 == level4)
            .Where(x => x.Level5 == level5)
            .Where(x => x.Address == address)
            .Where(x => x.Number == number);

    }
}
