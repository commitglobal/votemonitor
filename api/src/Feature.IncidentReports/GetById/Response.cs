using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Module.Answers.Models;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Module.Forms.Models;

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
    public SubmissionAttachmentModel[] Attachments { get; init; } = [];

    /// <summary>Moves of this report to another polling station by a platform admin, oldest first.</summary>
    public PollingStationCorrectionModel[] PollingStationCorrections { get; init; } = [];

    /// <summary>The caller NGO's own follow-up status (see /follow-up-statuses); FollowUpStatus is the record's fixed value.</summary>
    public Guid? FollowUpStatusId { get; init; }
    public string? FollowUpStatusName { get; init; }
    public string? FollowUpStatusColor { get; init; }
    public string? FollowUpStatusIcon { get; init; }
}
