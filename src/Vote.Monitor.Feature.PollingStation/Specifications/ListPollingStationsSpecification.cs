using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Feature.PollingStation.Specifications;

public class ListPollingStationsSpecification : Specification<Domain.Entities.PollingStationAggregate.PollingStation>
{
    public ListPollingStationsSpecification(string? addressFilter, Dictionary<string, string>? tagFilters, int pageSize, int page)
    {
        if (!string.IsNullOrEmpty(addressFilter))
        {
            Query
                .Where(x => EF.Functions.Like(x.Address, $"%{addressFilter}%"));
        }

        if (tagFilters is not null && tagFilters.Count != 0)
        {
            Query
                .Where(station => tagFilters.Count == 0 || EF.Functions.JsonContains(station.Tags, tagFilters));
        }

        Query
            .OrderBy(x => x.DisplayOrder);

        Query
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
