using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<PollingStationAggregate> ApplyOrdering(this ISpecificationBuilder<PollingStationAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(PollingStationAggregate.Address), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.Address)
                    .ThenBy(x => x.Id)
                : builder
                    .OrderByDescending(x => x.Address)
                    .ThenBy(x => x.Id);
        }

        if (string.Equals(request.SortColumnName, nameof(PollingStationAggregate.DisplayOrder), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.Id)
                : builder
                    .OrderByDescending(x => x.DisplayOrder)
                    .ThenBy(x => x.Address)
                    .ThenBy(x => x.Id);
        }

        return builder
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Address)
            .ThenBy(x => x.Id);
    }
}
