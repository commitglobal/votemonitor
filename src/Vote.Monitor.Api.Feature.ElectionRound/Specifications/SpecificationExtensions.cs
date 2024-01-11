using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<ElectionRoundAggregate> ApplyOrdering(this ISpecificationBuilder<ElectionRoundAggregate> builder, BaseFilterRequest filter)
    {
        // We want the "asc" to be the default, that's why the condition is reverted.
        var isAscending = !(filter.SortOrder?.Equals(SortOrder.Asc) ?? false);

        return filter.ColumnName switch
        {
            nameof(ElectionRoundAggregate.Name) => isAscending ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name),
            nameof(ElectionRoundAggregate.Status) => isAscending ? builder.OrderBy(x => x.Status) : builder.OrderByDescending(x => x.Name),
            _ => builder.OrderBy(x => x.Id)
        };
    }
}
