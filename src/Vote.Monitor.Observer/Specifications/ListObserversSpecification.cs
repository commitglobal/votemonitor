using Ardalis.Specification;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Observer.Specifications;

public class ListObserversSpecification : Specification<Domain.Entities.ApplicationUserAggregate.Observer>
{
    public ListObserversSpecification(string? nameFilter, UserStatus? status, int pageSize, int page)
    {
        if (!string.IsNullOrEmpty(nameFilter))
        {
            Query
                .Where(x => x.Name.StartsWith(nameFilter));
        }

        if (status != null)
        {
            Query
                .Where(x => x.Status == status);
        }

        Query
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
