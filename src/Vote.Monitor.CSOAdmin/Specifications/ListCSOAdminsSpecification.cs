namespace Vote.Monitor.CSOAdmin.Specifications;

public class ListCSOAdminsSpecification : Specification<CSOAdminAggregate>
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
