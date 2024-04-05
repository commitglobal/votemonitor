using Vote.Monitor.Core.Models;

namespace Feature.MonitoringObservers.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<MonitoringObserverAggregate> ApplyOrdering(this ISpecificationBuilder<MonitoringObserverAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(MonitoringObserverAggregate.Observer.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder.OrderBy(x => x.Observer.Name)
                : builder.OrderByDescending(x => x.Observer.Name);
        }

        if (string.Equals(request.SortColumnName, nameof(MonitoringObserverAggregate.Observer.Status), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder.OrderBy(x => x.Status)
                : builder.OrderByDescending(x => x.Status);
        }

        return builder
            .OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Observer.Name);
    }
}
