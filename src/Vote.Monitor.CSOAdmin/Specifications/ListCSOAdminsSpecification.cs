using Ardalis.Specification;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.CSOAdmin.Specifications;

public class ListCSOAdminsSpecification : Specification<Domain.Entities.ApplicationUserAggregate.CSOAdmin>
{
    public ListCSOAdminsSpecification(string? nameFilter, UserStatus? userStatus, int pageSize, int page)
    {
        if (!string.IsNullOrEmpty(nameFilter))
        {
            Query
                .Where(x => x.Name.StartsWith(nameFilter));
        }

        if (userStatus != null)
        {
            Query
                .Where(x => x.Status == userStatus);
        }

        Query
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
