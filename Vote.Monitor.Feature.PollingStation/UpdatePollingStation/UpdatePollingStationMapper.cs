using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Models;

namespace Vote.Monitor.Feature.PollingStation.UpdatePollingStation;
internal class UpdatePollingStationMapper : Mapper<PollingStationUpdateRequestDTO, PollingStationReadDto, PollingStationModel>
{
    public override PollingStationModel ToEntity(PollingStationUpdateRequestDTO source)
    {
        return new PollingStationModel()
        {
            Address = source.Address,
            DisplayOrder = source.DisplayOrder,
            Tags = source.Tags
        };
    }
}
