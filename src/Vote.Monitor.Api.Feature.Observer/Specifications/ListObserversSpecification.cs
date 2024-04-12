using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public sealed class ListObserversSpecification : Specification<ObserverAggregate>
{
    public ListObserversSpecification(List.Request request)
    {
        Query
            .Include(x => x.ApplicationUser)
            .Search(x => x.ApplicationUser.FirstName, "%" + request.NameFilter + "%", !string.IsNullOrEmpty(request.NameFilter))

            .Search(x => x.ApplicationUser.LastName, "%" + request.NameFilter + "%", !string.IsNullOrEmpty(request.NameFilter))
            .Where(x => x.ApplicationUser.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
