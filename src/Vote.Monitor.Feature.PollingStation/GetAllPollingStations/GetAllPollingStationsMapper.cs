using FastEndpoints;
using Vote.Monitor.Core;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;

namespace Vote.Monitor.Feature.PollingStation.GetAllPollingStations;
internal class GetAllPollingStationsMapper :IResponseMapper
{
    public PollingStationReadDto FromEntity(PollingStationModel source)
    {
        return new PollingStationReadDto()
        {
            Id = source.Id,
            Address = source.Address,
            DisplayOrder = source.DisplayOrder,
            Tags = source.TagsDictionary()
        };
    }

    public PaginationResponse<PollingStationReadDto> FromEntity(List<PollingStationModel> source, int currentPage, int pagesize, int totalItems, int totalPages )
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





