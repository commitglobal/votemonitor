using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.CSOAdmin;

public record CSOAdminModel
{
    public required Guid Id { get; init; }
    public required string Login { get; init; }
    public required string Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, int>))]
    public required UserStatus Status { get; init; }
}
