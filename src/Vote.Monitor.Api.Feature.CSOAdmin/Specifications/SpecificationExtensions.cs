using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.CSOAdmin.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<CSOAdminAggregate> ApplyOrdering(this ISpecificationBuilder<CSOAdminAggregate> builder, BaseFilterRequest filter)
    {
        // We want the "asc" to be the default, that's why the condition is reverted.
        var isAscending = !(filter.SortOrder?.Equals(SortOrder.Asc) ?? false);

        if (string.Equals(filter.ColumnName, nameof(CSOAdminAggregate.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscending ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name);
        }

        return builder.ApplyDefaultOrdering(filter);
    }
}
