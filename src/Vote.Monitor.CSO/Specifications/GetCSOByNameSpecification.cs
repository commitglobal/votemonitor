namespace Vote.Monitor.CSO.Specifications;

public class GetCSOByNameSpecification : Specification<Domain.Entities.CSOAggregate.CSO>
{
    public GetCSOByNameSpecification(string name)
    {
        Query
            .Where(x => x.Name == name);
    }
}
