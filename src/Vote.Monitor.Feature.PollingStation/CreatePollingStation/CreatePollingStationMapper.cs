using FastEndpoints;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;

namespace Vote.Monitor.Feature.PollingStation.CreatePollingStation;
internal class CreatePollingStationMapper : Mapper<PollingStationCreateRequestDto, PollingStationReadDto, Domain.Models.PollingStation>
{

    public override Domain.Models.PollingStation ToEntity(PollingStationCreateRequestDto source)
    {
        Domain.Models.PollingStation st = new()
        {
            Address = source.Address,
            DisplayOrder = source.DisplayOrder,
            Tags = source.Tags.ToTags()
        };

        return st;
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
