namespace Feature.Ngos.Specifications;

public sealed class GetNgoByNameSpecification : Specification<NgoAggregate>
{
    public GetNgoByNameSpecification(string name)
    {
        Query
            .Where(x => x.Name == name);
    }
}
