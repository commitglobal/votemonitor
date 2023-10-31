using FastEndpoints;
using Vote.Monitor.Core;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;

namespace Vote.Monitor.Feature.PollingStation.GetAllPollingStations;
internal class GetAllPollingStationsMapper :IResponseMapper
{
    public PollingStationReadDto FromEntity(Domain.Models.PollingStation source)
    {
        return new PollingStationReadDto()
        {
            Id = source.Id,
            Address = source.Address,
            DisplayOrder = source.DisplayOrder,
            Tags = source.Tags.ToDictionary()
        };
    }

    public PaginationResponse<PollingStationReadDto> FromEntity(List<Domain.Models.PollingStation> source, int currentPage, int pagesize, int totalItems, int totalPages )
    {
        return new PaginationResponse<PollingStationReadDto>()
        {
            CurrentPage = currentPage,
            PageSize = pagesize,
            TotalItems = totalItems,
            TotalPges = totalPages,
            Results =  source.Select(x => FromEntity(x)).ToList()
        };
    }
}





