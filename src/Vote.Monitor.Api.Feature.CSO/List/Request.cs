using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;

namespace Vote.Monitor.Api.Feature.CSO.List;

public class Request: BaseSortPaginatedRequest
{
    [QueryParam]
    public string? NameFilter { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<CSOStatus, string>))]
    public CSOStatus? Status { get; set; }
}
