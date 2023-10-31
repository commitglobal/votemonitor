using System.ComponentModel;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using FastEndpoints;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Vote.Monitor.ElectionRound.List;

public class Request
{
    [QueryParam]
    public string? NameFilter { get; set; }

    [QueryParam]
    public Guid? CountryId { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<ElectionRoundStatus, int>))]
    public ElectionRoundStatus? Status { get; set; }

    [QueryParam]
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [QueryParam]
    [DefaultValue(100)]
    public int PageSize { get; set; } = 100;
}
