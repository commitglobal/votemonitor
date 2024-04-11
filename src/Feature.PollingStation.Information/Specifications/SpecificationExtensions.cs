using Ardalis.Specification;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<PollingStationInformation> ApplyOrdering(this ISpecificationBuilder<PollingStationInformation> builder, BaseSortPaginatedRequest _)
    {
        return builder
            .OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Id);
    }
}
