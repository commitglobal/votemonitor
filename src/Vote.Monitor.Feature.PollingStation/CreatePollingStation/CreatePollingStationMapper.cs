using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Feature.PollingStation.CreatePollingStation;
internal class CreatePollingStationMapper : Mapper<PollingStationCreateRequestDto, PollingStationReadDto, Core.Models.PollingStationModel>
{

    public override PollingStationModel ToEntity(PollingStationCreateRequestDto source)
    {
        PollingStationModel st = new()
        {
            Address = source.Address,
            DisplayOrder = source.DisplayOrder
        };
        foreach (var tag in source.Tags)
        {
            st.Tags.Add(new TagModel()
            {
                Key = tag.Key,
                Value = tag.Value
            });
        }


        return st;
    }
    public override PollingStationReadDto FromEntity(PollingStationModel source)
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
