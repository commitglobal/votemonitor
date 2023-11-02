using Ardalis.Specification;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.CSOAdmin.Specifications;

public class ListCSOAdminsSpecification : Specification<Domain.Entities.ApplicationUserAggregate.CSOAdmin>
{
    public ListCSOAdminsSpecification(string? nameFilter, UserStatus? userStatus, int pageSize, int page)
    {
        Query
            .Search(x => x.Name, nameFilter, !string.IsNullOrEmpty(nameFilter))
            .Where(x => x.Status == userStatus)
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));

    }
}
