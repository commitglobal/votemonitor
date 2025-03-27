using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Get;

public record QuickReportDetailedModel
{
    public Guid Id { get; init; }
    public Guid ElectionRoundId { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportLocationType, string>))]
    public QuickReportLocationType QuickReportLocationType { get; init; }
    public DateTime Timestamp { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public Guid MonitoringObserverId { get; init; }
    public bool IsOwnObserver { get; init; }
    public string ObserverName { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string[] Tags { get; init; } = [];
    public Guid? PollingStationId { get; init; }
    public string? Level1 { get; init; }
    public string? Level2 { get; init; }
    public string? Level3 { get; init; }
    public string? Level4 { get; init; }
    public string? Level5 { get; init; }
    public string? Number { get; init; }
    public string? Address { get; init; }
    public string? PollingStationDetails { get; init; }
    
    [JsonConverter(typeof(SmartEnumNameConverter<IncidentCategory, string>))]
    public IncidentCategory IncidentCategory { get; init; } 

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportFollowUpStatus, string>))]
    public QuickReportFollowUpStatus FollowUpStatus { get; init; }
    public QuickReportAttachmentModel[] Attachments { get; init; }

    public static QuickReportDetailedModel FromEntity(QuickReport quickReport, IEnumerable<QuickReportAttachmentModel> attachments)
    {
        return new QuickReportDetailedModel
        {
            Id = quickReport.Id,
            ElectionRoundId = quickReport.ElectionRoundId,
            QuickReportLocationType = quickReport.QuickReportLocationType,
            Title = quickReport.Title,
            Description = quickReport.Description,
            MonitoringObserverId = quickReport.MonitoringObserverId,
            ObserverName = quickReport.MonitoringObserver.Observer.ApplicationUser.DisplayName,
            PollingStationId = quickReport.PollingStationId,
            Level1 = quickReport.PollingStation?.Level1,
            Level2 = quickReport.PollingStation?.Level2,
            Level3 = quickReport.PollingStation?.Level3,
            Level4 = quickReport.PollingStation?.Level4,
            Level5 = quickReport.PollingStation?.Level5,
            Number = quickReport.PollingStation?.Number,
            Address = quickReport.PollingStation?.Address,
            PollingStationDetails = quickReport.PollingStationDetails,
            Timestamp = quickReport.LastUpdatedAt,
            Attachments = attachments.ToArray(),
            FollowUpStatus = quickReport.FollowUpStatus,
            IncidentCategory = quickReport.IncidentCategory
        };
    }

}
