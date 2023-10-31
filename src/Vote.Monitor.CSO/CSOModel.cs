using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.CSO;

public record CSOModel
{
    public Guid Id { get; init; }
    public required string Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<CSOStatus, int>))]
    public required CSOStatus Status { get; init; }
}
