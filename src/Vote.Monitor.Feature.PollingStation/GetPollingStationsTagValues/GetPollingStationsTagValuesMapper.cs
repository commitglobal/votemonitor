using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastEndpoints;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStationsTagValues;
internal class GetPollingStationsTagValuesMapper : Mapper<TagValuesRequest, List<TagReadDto>, List<TagModel>>
{


    public override List<TagReadDto> FromEntity(List<TagModel> source)
    {
        List<TagReadDto> tagReadDtos = new();
        foreach (TagModel tagReadDto in source)
        {
            tagReadDtos.Add(new TagReadDto()
            {
                Key = tagReadDto.Key,
                Value = tagReadDto.Value
            });
        }
        return tagReadDtos;
    }
}
