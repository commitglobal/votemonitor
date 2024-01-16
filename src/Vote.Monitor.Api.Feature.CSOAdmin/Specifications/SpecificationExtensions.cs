namespace Vote.Monitor.Api.Feature.CSOAdmin.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<CSOAdminAggregate> ApplyOrdering(this ISpecificationBuilder<CSOAdminAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(CSOAdminAggregate.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name);
        }

        return builder
            .OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Name);
    }
}
