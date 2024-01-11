namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<PollingStationAggregate> ApplyOrdering(this ISpecificationBuilder<PollingStationAggregate> builder, BaseFilterRequest filter)
    {
        // We want the "asc" to be the default, that's why the condition is reverted.
        var isAscending = !(filter.SortOrder?.Equals(SortOrder.Asc) ?? false);

        return filter.ColumnName switch
        {
            nameof(PollingStationAggregate.Address) => isAscending ? builder.OrderBy(x => x.Address) : builder.OrderByDescending(x => x.Address),
            nameof(PollingStationAggregate.DisplayOrder) => isAscending ? builder.OrderBy(x => x.DisplayOrder) : builder.OrderByDescending(x => x.DisplayOrder),
            _ => builder.OrderBy(x => x.DisplayOrder)
        };
    }
}
