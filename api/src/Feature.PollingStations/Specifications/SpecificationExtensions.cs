using Vote.Monitor.Core.Models;

namespace Feature.PollingStations.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<PollingStationAggregate> ApplyOrdering(
        this ISpecificationBuilder<PollingStationAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(PollingStationAggregate.Level1),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.Level1)
                    .ThenBy(x => x.DisplayOrder)
                : builder
                    .OrderByDescending(x => x.Level1)
                    .ThenBy(x => x.DisplayOrder);
        }

        if (string.Equals(request.SortColumnName, nameof(PollingStationAggregate.Level2),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.Level2)
                    .ThenBy(x => x.DisplayOrder)
                : builder
                    .OrderByDescending(x => x.Level2)
                    .ThenBy(x => x.DisplayOrder);
        }

        if (string.Equals(request.SortColumnName, nameof(PollingStationAggregate.Level3),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.Level3)
                    .ThenBy(x => x.DisplayOrder)
                : builder
                    .OrderByDescending(x => x.Level3)
                    .ThenBy(x => x.DisplayOrder);
        }

        if (string.Equals(request.SortColumnName, nameof(PollingStationAggregate.Level4),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.Level4)
                    .ThenBy(x => x.DisplayOrder)
                : builder
                    .OrderByDescending(x => x.Level4)
                    .ThenBy(x => x.DisplayOrder);
        }

        if (string.Equals(request.SortColumnName, nameof(PollingStationAggregate.Level5),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.Level5)
                    .ThenBy(x => x.DisplayOrder)
                : builder
                    .OrderByDescending(x => x.Level5)
                    .ThenBy(x => x.DisplayOrder);
        }

        if (string.Equals(request.SortColumnName, nameof(PollingStationAggregate.Number),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.Number)
                    .ThenBy(x => x.DisplayOrder)
                : builder
                    .OrderByDescending(x => x.Number)
                    .ThenBy(x => x.DisplayOrder);
        }

        if (string.Equals(request.SortColumnName, nameof(PollingStationAggregate.Address),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.Address)
                    .ThenBy(x => x.DisplayOrder)
                : builder
                    .OrderByDescending(x => x.Address)
                    .ThenBy(x => x.DisplayOrder);
        }

        return builder
            .OrderBy(x => x.Level1)
            .ThenBy(x => x.Level2)
            .ThenBy(x => x.Level3)
            .ThenBy(x => x.Level4)
            .ThenBy(x => x.Level5)
            .ThenBy(x => x.Number)
            .ThenBy(x => x.DisplayOrder);
    }
}
