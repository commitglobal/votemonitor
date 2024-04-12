using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Feature.MonitoringObservers.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<MonitoringObserverAggregate> ApplyOrdering(this ISpecificationBuilder<MonitoringObserverAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(ApplicationUser.FirstName), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder.OrderBy(x => x.Observer.ApplicationUser.FirstName)
                : builder.OrderByDescending(x => x.Observer.ApplicationUser.FirstName);
        }

        if (string.Equals(request.SortColumnName, nameof(ApplicationUser.LastName), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder.OrderBy(x => x.Observer.ApplicationUser.LastName)
                : builder.OrderByDescending(x => x.Observer.ApplicationUser.LastName);
        }

        if (string.Equals(request.SortColumnName, nameof(MonitoringObserverAggregate.Observer.ApplicationUser.Status), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder.OrderBy(x => x.Status)
                : builder.OrderByDescending(x => x.Status);
        }

        return builder
            .OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Observer.ApplicationUser.FirstName);
    }
}
