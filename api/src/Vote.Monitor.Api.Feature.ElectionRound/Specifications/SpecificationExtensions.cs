using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<ElectionRoundAggregate> ApplyOrdering(this ISpecificationBuilder<ElectionRoundAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(ElectionRoundAggregate.Title), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder.OrderBy(x => x.Title).ThenByDescending(x=>x.StartDate)
                : builder.OrderByDescending(x => x.Title).ThenByDescending(x=>x.StartDate);
        }

        if (string.Equals(request.SortColumnName, nameof(ElectionRoundAggregate.Status), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder.OrderBy(x => x.Status).ThenByDescending(x=>x.StartDate)
                : builder.OrderByDescending(x => x.Status).ThenByDescending(x=>x.StartDate);
        }     
        
        if (string.Equals(request.SortColumnName, nameof(ElectionRoundAggregate.StartDate), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder.OrderBy(x => x.StartDate)
                : builder.OrderByDescending(x => x.StartDate);
        }

        return builder
            .OrderByDescending(x => x.StartDate)
            .ThenBy(x => x.Title);
    }
}
