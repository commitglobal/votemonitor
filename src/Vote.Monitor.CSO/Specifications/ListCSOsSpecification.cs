using Ardalis.Specification;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.CSO.Specifications;

public class ListCSOsSpecification : Specification<Domain.Entities.CSOAggregate.CSO>
{
    public ListCSOsSpecification(string? nameFilter, CSOStatus? csoStatus, int pageSize, int page)
    {
        Query
            .Search(x => x.Name, nameFilter, !string.IsNullOrEmpty(nameFilter))
            .Where(x => x.Status == csoStatus)
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
