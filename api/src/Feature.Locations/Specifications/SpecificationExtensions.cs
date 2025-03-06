using Vote.Monitor.Core.Models;

namespace Feature.Locations.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<LocationAggregate> ApplyOrdering(
        this ISpecificationBuilder<LocationAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(LocationAggregate.Level1),
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

        if (string.Equals(request.SortColumnName, nameof(LocationAggregate.Level2),
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

        if (string.Equals(request.SortColumnName, nameof(LocationAggregate.Level3),
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

        if (string.Equals(request.SortColumnName, nameof(LocationAggregate.Level4),
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

        if (string.Equals(request.SortColumnName, nameof(LocationAggregate.Level5),
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

        return builder
            .OrderBy(x => x.Level1)
            .ThenBy(x => x.Level2)
            .ThenBy(x => x.Level3)
            .ThenBy(x => x.Level4)
            .ThenBy(x => x.Level5)
            .ThenBy(x => x.DisplayOrder);
    }
}
