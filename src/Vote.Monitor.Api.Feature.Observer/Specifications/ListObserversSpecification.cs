namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public class ListObserversSpecification : Specification<ObserverAggregate>
{
    public ListObserversSpecification(string? nameFilter, UserStatus? status, int pageSize, int page)
    {
        Query
            .Search(x => x.Name, "%" + nameFilter + "%", !string.IsNullOrEmpty(nameFilter))
            .Where(x => x.Status == status, status != null)
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
