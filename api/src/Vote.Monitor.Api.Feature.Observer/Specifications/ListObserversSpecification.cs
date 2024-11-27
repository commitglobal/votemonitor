using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public sealed class ListObserversSpecification : Specification<ObserverAggregate>
{
    public ListObserversSpecification(List.Request request)
    {
        Query
            .Include(x => x.ApplicationUser)
            .Search(x => x.ApplicationUser.FirstName, $"%{request.SearchText?.Trim() ?? string.Empty}%", !string.IsNullOrEmpty(request.SearchText))

            .Search(x => x.ApplicationUser.LastName, $"%{request.SearchText?.Trim() ?? string.Empty}%", !string.IsNullOrEmpty(request.SearchText))
            .Where(x => x.ApplicationUser.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
