using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers;

public class MonitoringObserverModel
{
    public Guid Id { get; init; }
    public Guid InviterNgoId { get; set; }
    public Guid ObserverId { get; init; }
    public string Name { get; init; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public required IReadOnlyList<string> Tags { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public UserStatus UserStatus { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<MonitoringObserverStatus, string>))]
    public MonitoringObserverStatus MonitoringObserverStatus { get; set; }
}
