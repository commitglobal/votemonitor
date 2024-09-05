using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Feature.Monitoring.List;

public class Response
{
    public List<MonitoringNgoModel> MonitoringNgos { get; init; }
}

public class MonitoringNgoModel
{
    public Guid Id { get; init; }

    public required Guid NgoId { get; init; }
    public required string Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<NgoStatus, string>))]
    public required NgoStatus NgoStatus { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<MonitoringNgoStatus, string>))]
    public required MonitoringNgoStatus MonitoringNgoStatus { get; init; }
}
