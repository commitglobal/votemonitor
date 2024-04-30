using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports;

public record QuickReportModel
{
    public Guid Id { get; init; }
    public Guid ElectionRoundId { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportLocationType, string>))]
    public QuickReportLocationType QuickReportLocationType { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid? PollingStationId { set; get; }
    public string? PollingStationDetails { get; set; }
    public List<QuickReportAttachmentModel> Attachments { get; init; }

    public static QuickReportModel FromEntity(QuickReport quickReport, IEnumerable<QuickReportAttachmentModel> attachments)
    {
        return new QuickReportModel()
        {
            Id = quickReport.Id,
            ElectionRoundId = quickReport.ElectionRoundId,
            QuickReportLocationType = quickReport.QuickReportLocationType,
            Title = quickReport.Title,
            Description = quickReport.Description,
            PollingStationId = quickReport.PollingStationId,
            PollingStationDetails = quickReport.PollingStationDetails,
            Attachments = attachments.ToList()
        };
    }
}
