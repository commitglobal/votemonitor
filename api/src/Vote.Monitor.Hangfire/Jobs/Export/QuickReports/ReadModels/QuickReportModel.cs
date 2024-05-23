using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Vote.Monitor.Hangfire.Jobs.Export.QuickReports.ReadModels;

public class QuickReportModel
{
    public Guid Id { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportLocationType, string>))]
    public QuickReportLocationType QuickReportLocationType { get; set; }
    public DateTime Timestamp { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid MonitoringObserverId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? PollingStationDetails { get; set; }
    public Guid? PollingStationId { get; set; }
    public string? Level1 { get; set; }
    public string? Level2 { get; set; }
    public string? Level3 { get; set; }
    public string? Level4 { get; set; }
    public string? Level5 { get; set; }
    public string? Number { get; set; }
    public string? Address { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportFollowUpStatus, string>))]
    public QuickReportFollowUpStatus FollowUpStatus { get; set; }
    public QuickReportAttachmentModel[] Attachments { get; init; }
}
