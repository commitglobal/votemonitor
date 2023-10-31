using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Observer;

public record ObserverModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Login { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, int>))]
    public UserStatus Status { get; init; }
}
