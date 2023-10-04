using FastEndpoints;
using Vote.Monitor.Core.Models;

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
            Tags = source.TagsDictionary()
        };
    }
}

