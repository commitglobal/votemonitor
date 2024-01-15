namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<PollingStationAggregate> ApplyOrdering(this ISpecificationBuilder<PollingStationAggregate> builder, BaseFilterRequest filter)
    {
        // We want the "asc" to be the default, that's why the condition is reverted.
        var isAscending = !(filter.SortOrder?.Equals(SortOrder.Desc) ?? false);

        if (string.Equals(filter.ColumnName, nameof(PollingStationAggregate.Address), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscending ? builder.OrderBy(x => x.Address) : builder.OrderByDescending(x => x.Address);
        }

        if (string.Equals(filter.ColumnName, nameof(PollingStationAggregate.DisplayOrder), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscending
                ? builder.OrderBy(x => x.DisplayOrder)
                : builder.OrderByDescending(x => x.DisplayOrder);
        }

        return builder.OrderBy(x => x.DisplayOrder);
    }
}
