namespace Vote.Monitor.Api.Feature.CSO.Specifications;

public class ListCSOsSpecification : Specification<CSOAggregate>
{
    public ListCSOsSpecification(string? nameFilter, CSOStatus? csoStatus, int pageSize, int page)
    {
        Query
            .Search(x => x.Name, "%" + nameFilter + "%", !string.IsNullOrEmpty(nameFilter))
            .Where(x => x.Status == csoStatus)
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
