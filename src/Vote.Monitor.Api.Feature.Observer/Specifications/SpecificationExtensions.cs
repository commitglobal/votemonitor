namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<ObserverAggregate> ApplyOrdering(this ISpecificationBuilder<ObserverAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(ObserverAggregate.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id)
                : builder
                    .OrderByDescending(x => x.Name)
                    .ThenBy(x => x.Id);
        }

        if (string.Equals(request.SortColumnName, nameof(ObserverAggregate.Status), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.Status)
                    .ThenBy(x => x.Id)
                : builder
                    .OrderByDescending(x => x.Status)
                    .ThenBy(x => x.Id);
        }

        return builder.OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Id);
    }
}
