using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public class ListObserversSpecification : Specification<ObserverAggregate>
{
    public ListObserversSpecification(List.Request request)
    {
        Query
            .Search(x => x.Name, "%" + request.NameFilter + "%", !string.IsNullOrEmpty(request.NameFilter))
            .Where(x => x.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
