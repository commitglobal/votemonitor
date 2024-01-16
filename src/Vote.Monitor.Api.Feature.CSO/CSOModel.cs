using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;

namespace Vote.Monitor.Api.Feature.CSO;

public record CSOModel
{
    public Guid Id { get; init; }
    public required string Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<CSOStatus, string>))]
    public required CSOStatus Status { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
}
