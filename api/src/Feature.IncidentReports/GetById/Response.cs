using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Form.Module.Models;

namespace Feature.IncidentReports.GetById;

public class Response
{
    public Guid IncidentReportId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid FormId { get; init; }
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; }
    public string FormCode { get; init; }
    public TranslatedString FormName { get; init; }
    public string FormDefaultLanguage { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportFollowUpStatus, string>))]
    public IncidentReportFollowUpStatus FollowUpStatus { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportLocationType, string>))]
    public IncidentReportLocationType LocationType { get; init; }

    public bool IsCompleted { get; init; }

    public Guid? PollingStationId { get; init; }
    public string? PollingStationLevel1 { get; init; }
    public string? PollingStationLevel2 { get; init; }
    public string? PollingStationLevel3 { get; init; }
    public string? PollingStationLevel4 { get; init; }
    public string? PollingStationLevel5 { get; init; }
    public string? PollingStationNumber { get; init; }
    public string? LocationDescription { get; init; }

    public BaseQuestionModel[] Questions { get; init; } = [];
    public BaseAnswerModel[] Answers { get; init; } = [];
    public NoteModel[] Notes { get; init; } = [];
    public AttachmentModel[] Attachments { get; init; } = [];
}