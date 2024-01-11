namespace Vote.Monitor.Api.Feature.CSOAdmin.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<CSOAdminAggregate> ApplyOrdering(this ISpecificationBuilder<CSOAdminAggregate> builder, BaseFilterRequest filter)
    {
        // We want the "asc" to be the default, that's why the condition is reverted.
        var isAscending = !(filter.SortOrder?.Equals(SortOrder.Asc) ?? false);

        return filter.ColumnName switch
        {
            nameof(CSOAdminAggregate.Name) => isAscending ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name),
            _ => builder.OrderBy(x => x.Id)
        };
    }
}
