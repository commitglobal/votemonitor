using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.Ngo.Specifications;

public sealed class ListNgosSpecification : Specification<NgoAggregate>
{
    public ListNgosSpecification(List.Request request)
    {
        Query
            .Search(x => x.Name, "%" + request.SearchText + "%", !string.IsNullOrEmpty(request.SearchText))
            .Where(x => x.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
