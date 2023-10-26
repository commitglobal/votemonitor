using Ardalis.Specification;

namespace Vote.Monitor.CSO.Specifications;

public class GetCSOByName : Specification<Domain.Entities.CSOAggregate.CSO>
{
    public GetCSOByName(string name)
    {
        Query
            .Where(x => x.Name == name);
    }
}
