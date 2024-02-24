using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.ElectionRound;

public record ElectionRoundModel : ElectionRoundBaseModel
{
    public required List<MonitoringNgoModel> MonitoringNgos { get; init; }
    public required List<MonitoringObserverModel> MonitoringObservers { get; init; }
}

public class MonitoringNgoModel
{
    public required Guid NgoId { get; init; }
    public required string Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<NgoStatus, string>))]
    public required NgoStatus Status { get; init; }
}

public class MonitoringObserverModel
{
    public required Guid ObserverId { get; init; }
    public required string Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public required UserStatus Status { get; init; }
}
