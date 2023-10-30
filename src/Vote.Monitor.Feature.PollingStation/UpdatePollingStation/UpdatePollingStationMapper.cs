using FastEndpoints;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;

namespace Vote.Monitor.Feature.PollingStation.UpdatePollingStation;
internal class UpdatePollingStationMapper : Mapper<PollingStationUpdateRequestDTO, PollingStationReadDto, Domain.Models.PollingStation>
{
    public override Domain.Models.PollingStation ToEntity(PollingStationUpdateRequestDTO source)
    {
        var model =  new Domain.Models.PollingStation()
        {
            Address = source.Address,
            DisplayOrder = source.DisplayOrder,
            Tags = source.Tags.ToTags()           
        };
        
        return model;
    }

    public override PollingStationReadDto FromEntity(Domain.Models.PollingStation source)
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
