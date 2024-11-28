using System.Text.Json.Serialization;

namespace SubmissionsFaker.Models;

public class ListMonitoringNgos
{
    public List<MonitoringNgoModel> MonitoringNgos { get; init; }
}

public class MonitoringNgoModel
{
    public Guid Id { get; init; }

    public required Guid NgoId { get; init; }
    public required string Name { get; init; }
}
