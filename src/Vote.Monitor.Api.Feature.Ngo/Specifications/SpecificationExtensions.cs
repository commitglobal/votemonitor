namespace Vote.Monitor.Api.Feature.Ngo.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<NgoAggregate> ApplyOrdering(this ISpecificationBuilder<NgoAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(NgoAggregate.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name);
        }

        return builder.OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Name);
    }
}
