using FastEndpoints;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStation;

internal  class GetPollingStationMapper :IResponseMapper
{
    public  PollingStationReadDto FromEntity(Domain.Models.PollingStation source)
    {
        return new PollingStationReadDto()
        {
            Id = source.Id,
            Address = source.Address,
            DisplayOrder = source.DisplayOrder,
            Tags = source.Tags.ToDictionary()
        };
    }
}

