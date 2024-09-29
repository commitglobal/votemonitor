using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Hangfire.Models;

namespace Vote.Monitor.Hangfire.Jobs.Export.IssueReports.ReadModels;

public class IssueReportModel
{
    public Guid IssueReportId { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<IssueReportFollowUpStatus, string>))]
    public IssueReportFollowUpStatus FollowUpStatus { get; set; } = IssueReportFollowUpStatus.NotApplicable;

    public Guid FormId { get; set; }
    public DateTime TimeSubmitted { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<IssueReportLocationType, string>))]
    public IssueReportLocationType LocationType { get; set; } = IssueReportLocationType.OtherLocation;

    public string? LocationDescription { get; set; } = string.Empty;
    public string? Level1 { get; set; } = string.Empty;
    public string? Level2 { get; set; } = string.Empty;
    public string? Level3 { get; set; } = string.Empty;
    public string? Level4 { get; set; } = string.Empty;
    public string? Level5 { get; set; } = string.Empty;
    public string? Number { get; set; } = string.Empty;
    public Guid MonitoringObserverId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public BaseAnswer[] Answers { get; set; } = [];
    public SubmissionNoteModel[] Notes { get; set; } = [];
    public SubmissionAttachmentModel[] Attachments { get; set; } = [];
}