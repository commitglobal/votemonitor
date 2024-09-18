namespace Feature.Locations.Specifications;

public sealed class GetLocationByIdSpecification : Specification<LocationAggregate>,
    ISingleResultSpecification<LocationAggregate>
{
    public GetLocationByIdSpecification(Guid electionRoundId, Guid id)
    {
        Query.Where(location => location.ElectionRoundId == electionRoundId && location.Id == id);
    }
}
