using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.GetMonitoringObservers;

public class Response
{
    public List<MonitoringObserverModel> MonitoringObservers { get; init; }
}

public class MonitoringObserverModel
{
    public Guid Id { get; init; }
    public Guid InviterNgoId { get; set; }
    public required Guid ObserverId { get; init; }
    public required string Name { get; init; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public required UserStatus UserStatus { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<MonitoringObserverStatus, string>))]
    public required MonitoringObserverStatus MonitoringObserverStatus { get; set; }
}
