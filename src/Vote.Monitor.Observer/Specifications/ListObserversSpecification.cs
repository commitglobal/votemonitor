using Ardalis.Specification;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Observer.Specifications;

public class ListObserversSpecification : Specification<Domain.Entities.ApplicationUserAggregate.Observer>
{
    public ListObserversSpecification(string? nameFilter, UserStatus? status, int pageSize, int page)
    {

        Query
            .Search(x => x.Name, nameFilter, !string.IsNullOrEmpty(nameFilter))
            .Where(x => x.Status == status, status != null)
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
