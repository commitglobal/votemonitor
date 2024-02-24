namespace Vote.Monitor.Api.Feature.NgoAdmin.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<NgoAdminAggregate> ApplyOrdering(this ISpecificationBuilder<NgoAdminAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(NgoAdminAggregate.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name);
        }

        return builder
            .OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Name);
    }
}
