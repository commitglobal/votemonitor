namespace Vote.Monitor.Api.Feature.CSO.Specifications;

public class GetCSOByNameSpecification : Specification<CSOAggregate>
{
    public GetCSOByNameSpecification(string name)
    {
        Query
            .Where(x => x.Name == name);
    }
}
