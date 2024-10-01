using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.ListByObserver;

public record ObserverIncidentReportsOverview
{
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string PhoneNumber { get; init; } = default!;
    public string[] Tags { get; init; } = [];
    public int NumberOfFlaggedAnswers { get; init; }
    public int NumberOfIncidentsSubmitted { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportFollowUpStatus, string>))]
    public IncidentReportFollowUpStatus? FollowUpStatus { get; init; }
}
