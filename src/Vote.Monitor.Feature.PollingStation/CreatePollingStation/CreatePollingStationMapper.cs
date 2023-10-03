using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Models;

namespace Vote.Monitor.Feature.PollingStation.CreatePollingStation;
internal class CreatePollingStationMapper : Mapper<PollingStationCreateRequestDto, PollingStationReadDto, PollingStationModel>
{

    public override PollingStationModel ToEntity(PollingStationCreateRequestDto source)
    {
        return new PollingStationModel()
        {
            Address = source.Address,
            DisplayOrder = source.DisplayOrder,
            Tags = source.Tags
        };
    }
    public override PollingStationReadDto FromEntity(PollingStationModel source)
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
