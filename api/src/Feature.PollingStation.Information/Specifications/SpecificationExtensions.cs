using Ardalis.Specification;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<PollingStationInformation> ApplyOrdering(this ISpecificationBuilder<PollingStationInformation> builder, BaseSortPaginatedRequest _)
    {
        return builder
            .OrderByDescending(x => x.LastUpdatedAt)
            .ThenBy(x => x.Id);
    }
}
