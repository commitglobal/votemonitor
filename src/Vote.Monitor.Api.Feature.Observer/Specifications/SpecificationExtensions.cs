using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<ObserverAggregate> ApplyOrdering(this ISpecificationBuilder<ObserverAggregate> builder, BaseFilterRequest filter)
    {
        // We want the "asc" to be the default, that's why the condition is reverted.
        var isAscending = !(filter.SortOrder?.Equals(SortOrder.Asc) ?? false);

        return filter.ColumnName switch
        {
            nameof(ObserverAggregate.Name) => isAscending ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name),
            nameof(ObserverAggregate.Status) => isAscending ? builder.OrderBy(x => x.Status) : builder.OrderByDescending(x => x.Status),
            _ => builder.OrderBy(x => x.Id)
        };
    }
}
