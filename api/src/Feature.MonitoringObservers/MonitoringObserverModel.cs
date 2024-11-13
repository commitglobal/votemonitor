using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers;

public class MonitoringObserverModel
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string DisplayName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public string[] Tags { get; init; }
    public DateTime? LatestActivityAt { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<MonitoringObserverStatus, string>))]
    public MonitoringObserverStatus Status { get; init; }
}


