namespace Vote.Monitor.Api.Feature.CSO.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<CSOAggregate> ApplyOrdering(this ISpecificationBuilder<CSOAggregate> builder, BaseFilterRequest filter)
    {
        // We want the "asc" to be the default, that's why the condition is reverted.
        var isAscending = !(filter.SortOrder?.Equals(SortOrder.Asc) ?? false);

        return filter.ColumnName switch
        {
            nameof(CSOAggregate.Name) => isAscending ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name),
            _ => builder.OrderBy(x => x.Id)
        };
    }
}
