namespace Feature.Locations.Specifications;

public sealed class GetLocationByIdReadOnlySpecification : Specification<LocationAggregate>,
    ISingleResultSpecification<LocationAggregate>
{
    public GetLocationByIdReadOnlySpecification(Guid electionRoundId, Guid id)
    {
        Query.AsNoTracking();

        Query.Where(location => location.ElectionRoundId == electionRoundId && location.Id == id);
    }
}
