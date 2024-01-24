namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<ElectionRoundAggregate> ApplyOrdering(this ISpecificationBuilder<ElectionRoundAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(ElectionRoundAggregate.Title), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder.OrderBy(x => x.Title)
                : builder.OrderByDescending(x => x.Title);
        }

        if (string.Equals(request.SortColumnName, nameof(ElectionRoundAggregate.Status), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder.OrderBy(x => x.Status)
                : builder.OrderByDescending(x => x.Status);
        }

        return builder
            .OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Title);
    }
}
