using FastEndpoints;
using Vote.Monitor.Core;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStationsTagValues;
internal class GetPollingStationsTagValuesMapper : Mapper<TagValuesRequest, List<TagReadDto>, List<TagModel>>
{


    public override List<TagReadDto> FromEntity(List<TagModel> source)
    {
        return source.Select(x => new TagReadDto()
        {
            Key = x.Key,
            Value = x.Value
        })
            .ToList();
    }
}
