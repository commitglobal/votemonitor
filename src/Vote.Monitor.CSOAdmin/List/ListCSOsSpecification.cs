using Ardalis.Specification;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.CSOAdmin.List;

public class ListCSOsSpecification : Specification<Domain.Entities.ApplicationUserAggregate.CSOAdmin>
{
    public ListCSOsSpecification(string nameFilter, CSOStatus? csoStatus, int pageSize, int page)
    {
        if (!string.IsNullOrEmpty(nameFilter))
        {
            Query
                .Where(x => x.Name.StartsWith(nameFilter));
        }

        if (csoStatus != null)
        {
            Query
                .Where(x => x.Status == csoStatus);
        }

        Query
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
