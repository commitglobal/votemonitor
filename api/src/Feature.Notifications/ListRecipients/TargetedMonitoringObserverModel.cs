using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notifications.ListRecipients;

public class TargetedMonitoringObserverModel
{
    public Guid Id { get; init; }
    public string ObserverName { get; init; }

    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string[] Tags { get; init; } = [];

    [JsonConverter(typeof(SmartEnumNameConverter<MonitoringObserverStatus, string>))]
    public MonitoringObserverStatus Status { get; init; }
}
