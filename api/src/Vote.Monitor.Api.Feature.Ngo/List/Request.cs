using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Ngo.List;

public class Request: BaseSortPaginatedRequest
{
    [QueryParam]
    public string? SearchText { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<NgoStatus, string>))]
    public NgoStatus? Status { get; set; }
}
