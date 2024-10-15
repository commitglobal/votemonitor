using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports;

public record QuickReportModel
{
    public Guid Id { get; init; }
    public Guid ElectionRoundId { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportLocationType, string>))]
    public QuickReportLocationType QuickReportLocationType { get; init; }

    public DateTime Timestamp { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public Guid MonitoringObserverId { get; init; }
    public Guid? PollingStationId { get; init; }
    public string? PollingStationDetails { get; init; }
    public List<QuickReportAttachmentModel> Attachments { get; init; }
    public IncidentCategory IncidentCategory { get; init; }

    public static QuickReportModel FromEntity(QuickReport quickReport,
        IEnumerable<QuickReportAttachmentModel> attachments)
    {
        return new QuickReportModel
        {
            Id = quickReport.Id,
            ElectionRoundId = quickReport.ElectionRoundId,
            QuickReportLocationType = quickReport.QuickReportLocationType,
            Title = quickReport.Title,
            Description = quickReport.Description,
            MonitoringObserverId = quickReport.MonitoringObserverId,
            PollingStationId = quickReport.PollingStationId,
            PollingStationDetails = quickReport.PollingStationDetails,
            Timestamp = quickReport.LastModifiedOn ?? quickReport.CreatedOn,
            Attachments = attachments.ToList(),
            IncidentCategory = quickReport.IncidentCategory
        };
    }
}