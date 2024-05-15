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
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public Guid? PollingStationId { get; init; }
    public string? Level1 { get; init; }
    public string? Level2 { get; init; }
    public string? Level3 { get; init; }
    public string? Level4 { get; init; }
    public string? Level5 { get; init; }
    public string? Number { get; init; }
    public string? Address { get; init; }
    public string? PollingStationDetails { get; init; }
    public List<QuickReportAttachmentModel> Attachments { get; init; }

    public static QuickReportModel FromEntity(QuickReport quickReport, IEnumerable<QuickReportAttachmentModel> attachments)
    {
        return new QuickReportModel
        {
            Id = quickReport.Id,
            ElectionRoundId = quickReport.ElectionRoundId,
            QuickReportLocationType = quickReport.QuickReportLocationType,
            Title = quickReport.Title,
            Description = quickReport.Description,
            MonitoringObserverId = quickReport.MonitoringObserverId,
            FirstName = quickReport.MonitoringObserver.Observer.ApplicationUser.FirstName,
            LastName = quickReport.MonitoringObserver.Observer.ApplicationUser.LastName,
            PollingStationId = quickReport.PollingStationId,
            Level1 = quickReport.PollingStation?.Level1,
            Level2 = quickReport.PollingStation?.Level2,
            Level3 = quickReport.PollingStation?.Level3,
            Level4 = quickReport.PollingStation?.Level4,
            Level5 = quickReport.PollingStation?.Level5,
            Number = quickReport.PollingStation?.Number,
            Address = quickReport.PollingStation?.Address,
            PollingStationDetails = quickReport.PollingStationDetails,
            Timestamp = quickReport.LastModifiedOn ?? quickReport.CreatedOn,
            Attachments = attachments.ToList()
        };
    }

}
