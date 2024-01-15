namespace Vote.Monitor.Api.Feature.CSO.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<CSOAggregate> ApplyOrdering(this ISpecificationBuilder<CSOAggregate> builder, BaseFilterRequest filter)
    {
        // We want the "asc" to be the default, that's why the condition is reverted.
        var isAscending = !(filter.SortOrder?.Equals(SortOrder.Desc) ?? false);

        if (string.Equals(filter.ColumnName, nameof(CSOAggregate.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscending ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name);
        }

        return builder.OrderBy(x => x.CreatedOn);
    }
}
