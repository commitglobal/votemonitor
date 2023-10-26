using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.CSOAdmin;

public record CSOAdminModel
{
    public string Name { get; init; }
    public string Login { get; init; }
    public string Password { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<CSOStatus, int>))]
    public UserStatus Status { get; init; }
}
