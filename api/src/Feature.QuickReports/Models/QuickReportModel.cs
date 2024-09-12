using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Models;

public record QuickReportModel
{
    public Guid Id { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportLocationType, string>))]
    public QuickReportLocationType QuickReportLocationType { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportIssueType, string>))]
    public QuickReportIssueType IssueType { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportOfficialComplaintFilingStatus, string>))]
    public QuickReportOfficialComplaintFilingStatus OfficialComplaintFilingStatus { get; set; }

    public DateTime Timestamp { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public Guid MonitoringObserverId { get; init; }
    public Guid? PollingStationId { get; init; }
    public string? PollingStationDetails { get; init; }
    public List<QuickReportAttachmentModel> Attachments { get; init; }

    public static QuickReportModel FromEntity(QuickReport quickReport,
        IEnumerable<QuickReportAttachmentModel> attachments)
    {
        return new QuickReportModel
        {
            Id = quickReport.Id,
            QuickReportLocationType = quickReport.QuickReportLocationType,
            Title = quickReport.Title,
            Description = quickReport.Description,
            MonitoringObserverId = quickReport.MonitoringObserverId,
            PollingStationId = quickReport.PollingStationId,
            PollingStationDetails = quickReport.PollingStationDetails,
            Timestamp = quickReport.LastModifiedOn ?? quickReport.CreatedOn,
            IssueType = quickReport.IssueType,
            OfficialComplaintFilingStatus = quickReport.OfficialComplaintFilingStatus,
            Attachments = attachments.ToList()
        };
    }
}