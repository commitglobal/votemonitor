namespace Feature.Ngos.Specifications;

public sealed class GetNgoWithSameNameSpecification : Specification<NgoAggregate>
{
    public GetNgoWithSameNameSpecification(Guid id, string name)
    {
        Query
            .Where(x => x.Id != id && x.Name == name);
    }
}
