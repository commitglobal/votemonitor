namespace Vote.Monitor.Api.Feature.Ngo.Specifications;

public sealed class GetNgoByNameSpecification : Specification<NgoAggregate>
{
    public GetNgoByNameSpecification(string name)
    {
        Query
            .Where(x => x.Name == name);
    }
}
