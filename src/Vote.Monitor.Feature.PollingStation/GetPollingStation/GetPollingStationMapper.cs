using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.Models;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStation;

internal  class GetPollingStationMapper :IResponseMapper
{
    public  PollingStationReadDto FromEntity(PollingStationModel source)
    {
        return new PollingStationReadDto()
        {
            Id = source.Id,
            Address = source.Address,
            DisplayOrder = source.DisplayOrder,
            Tags = source.Tags
        };
    }
}
