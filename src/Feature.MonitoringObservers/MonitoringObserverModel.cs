using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers;

public class MonitoringObserverModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public required IReadOnlyList<string> Tags { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<MonitoringObserverStatus, string>))]
    public MonitoringObserverStatus Status { get; init; }

    public static MonitoringObserverModel FromEntity(MonitoringObserverAggregate entity)
    {
        return new MonitoringObserverModel
        {
            Id = entity.Id,
            Email = entity.Observer.Login,
            Status = entity.Status,
            Name = entity.Observer.Name,
            PhoneNumber = entity.Observer.PhoneNumber,
            Tags = entity.Tags
        };
    }
}
