namespace Feature.Locations.Specifications;

public sealed class GetLocationSpecification : Specification<LocationAggregate>
{
    public GetLocationSpecification(Guid electionRoundId, string level1, string level2, string level3, string level4,
        string level5)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => x.Level1 == level1)
            .Where(x => x.Level2 == level2)
            .Where(x => x.Level3 == level3)
            .Where(x => x.Level4 == level4)
            .Where(x => x.Level5 == level5);
    }
}