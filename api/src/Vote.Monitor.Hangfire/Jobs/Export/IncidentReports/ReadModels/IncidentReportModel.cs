using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Hangfire.Models;

namespace Vote.Monitor.Hangfire.Jobs.Export.IncidentReports.ReadModels;

public class IncidentReportModel
{
    public Guid IncidentReportId { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportFollowUpStatus, string>))]
    public IncidentReportFollowUpStatus FollowUpStatus { get; set; } = IncidentReportFollowUpStatus.NotApplicable;

    public Guid FormId { get; set; }
    public DateTime TimeSubmitted { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportLocationType, string>))]
    public IncidentReportLocationType LocationType { get; set; } = IncidentReportLocationType.OtherLocation;

    public string? LocationDescription { get; set; } = string.Empty;
    public string? Level1 { get; set; } = string.Empty;
    public string? Level2 { get; set; } = string.Empty;
    public string? Level3 { get; set; } = string.Empty;
    public string? Level4 { get; set; } = string.Empty;
    public string? Level5 { get; set; } = string.Empty;
    public string? Number { get; set; } = string.Empty;
    public string NgoName { get; set; } = default!;
    public Guid MonitoringObserverId { get; set; }
    public string DisplayName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public bool IsCompleted { get; set; } = default!;
    public BaseAnswer[] Answers { get; set; } = [];
    public SubmissionNoteModel[] Notes { get; set; } = [];
    public SubmissionAttachmentModel[] Attachments { get; set; } = [];
}
