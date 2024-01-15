namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<ElectionRoundAggregate> ApplyOrdering(this ISpecificationBuilder<ElectionRoundAggregate> builder, BaseFilterRequest filter)
    {
        // We want the "asc" to be the default, that's why the condition is reverted.
        var isAscending = !(filter.SortOrder?.Equals(SortOrder.Desc) ?? false);

        if (string.Equals(filter.ColumnName, nameof(ElectionRoundAggregate.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscending ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name);
        }

        if (string.Equals(filter.ColumnName, nameof(ElectionRoundAggregate.Status), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscending ? builder.OrderBy(x => x.Status) : builder.OrderByDescending(x => x.Status);
        }

        return builder.OrderBy(x => x.CreatedOn);
    }
}
