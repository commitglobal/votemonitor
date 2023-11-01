using System.ComponentModel;
using Ardalis.SmartEnum.SystemTextJson;
using System.Text.Json.Serialization;
using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.CSO.List;

public class Request
{
    [QueryParam]
    public string? NameFilter { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<CSOStatus, int>))]
    public CSOStatus? Status { get; set; }

    [QueryParam]
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [QueryParam]
    [DefaultValue(100)]
    public int PageSize { get; set; } = 100;
}
