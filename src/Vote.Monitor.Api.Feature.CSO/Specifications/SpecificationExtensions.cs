namespace Vote.Monitor.Api.Feature.CSO.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<CSOAggregate> ApplyOrdering(this ISpecificationBuilder<CSOAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(CSOAggregate.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name);
        }

        return builder.OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Name);
    }
}
