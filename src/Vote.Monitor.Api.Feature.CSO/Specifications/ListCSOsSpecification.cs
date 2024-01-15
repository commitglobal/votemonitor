using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.CSO.Specifications;

public class ListCSOsSpecification : Specification<CSOAggregate>
{
    public ListCSOsSpecification(List.Request request)
    {
        Query
            .Search(x => x.Name, "%" + request.NameFilter + "%", !string.IsNullOrEmpty(request.NameFilter))
            .Where(x => x.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
