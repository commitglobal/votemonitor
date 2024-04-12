using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers;

public class MonitoringObserverModel
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
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
            Email = entity.Observer.ApplicationUser.Email,
            Status = entity.Status,
            FirstName = entity.Observer.ApplicationUser.FirstName,
            LastName = entity.Observer.ApplicationUser.LastName,
            PhoneNumber = entity.Observer.ApplicationUser.PhoneNumber,
            Tags = entity.Tags
        };
    }
}
