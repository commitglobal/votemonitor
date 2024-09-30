using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Feature.IssueReports.Models;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.Form.Module.Models;

namespace Feature.IssueReports.GetById;

public class Response
{
    public Guid IssueReportId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid FormId { get; init; }
    public string FormCode { get; init; }
    public TranslatedString FormName { get; init; }
    public string FormDefaultLanguage { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IssueReportFollowUpStatus, string>))]
    public IssueReportFollowUpStatus FollowUpStatus { get; init; }
    
    [JsonConverter(typeof(SmartEnumNameConverter<IssueReportLocationType, string>))]
    public IssueReportLocationType LocationType { get; init; }

    public string? PollingStationId { get; init; }
    public string? PollingStationLevel1 { get; init; }
    public string? PollingStationLevel2 { get; init; }
    public string? PollingStationLevel3 { get; init; }
    public string? PollingStationLevel4 { get; init; }
    public string? PollingStationLevel5 { get; init; }
    public string? LocationDescription { get; init; }

    public BaseQuestionModel[] Questions { get; init; } = [];
    public BaseAnswerModel[] Answers { get; init; } = [];
    public NoteModel[] Notes { get; init; } = [];
    public AttachmentModel[] Attachments { get; init; } = [];
}