using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Module.Answers.Models;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Module.Forms.Models;

namespace Feature.IncidentReports.GetById;

public record Response
{
    public Guid IncidentReportId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid FormId { get; init; }
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; }
    public string Email { get; init; }

    /// <summary>The NGO whose observer sent the report.</summary>
    public string NgoName { get; init; }

    public string FormCode { get; init; }
    public TranslatedString FormName { get; init; }
    public string FormDefaultLanguage { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportFollowUpStatus, string>))]
    public IncidentReportFollowUpStatus? FollowUpStatus { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportLocationType, string>))]
    public IncidentReportLocationType LocationType { get; init; }

    public bool IsCompleted { get; init; }

    /// <summary>The questions the observer was shown (display logic applied) that they answered.</summary>
    public int? NumberOfQuestionsAnswered { get; init; }

    /// <summary>The questions the observer was shown (display logic applied) and left unanswered.</summary>
    public int? NumberOfUnansweredQuestions { get; init; }

    public Guid? PollingStationId { get; init; }
    public string? PollingStationLevel1 { get; init; }
    public string? PollingStationLevel2 { get; init; }
    public string? PollingStationLevel3 { get; init; }
    public string? PollingStationLevel4 { get; init; }
    public string? PollingStationLevel5 { get; init; }
    public string? PollingStationNumber { get; init; }
    public string? PollingStationAddress { get; init; }
    public string? LocationDescription { get; init; }

    public BaseQuestionModel[]? Questions { get; init; } = [];
    public BaseAnswerModel[]? Answers { get; init; } = [];
    public NoteModel[]? Notes { get; init; } = [];
    public SubmissionAttachmentModel[]? Attachments { get; init; } = [];

    /// <summary>Moves of this report to another polling station by a platform admin, oldest first.</summary>
    public PollingStationCorrectionModel[] PollingStationCorrections { get; init; } = [];

    /// <summary>The caller NGO's own follow-up status (see /follow-up-statuses); FollowUpStatus is the record's fixed value.</summary>
    public Guid? FollowUpStatusId { get; init; }
    public string? FollowUpStatusName { get; init; }
    public string? FollowUpStatusColor { get; init; }
    public string? FollowUpStatusIcon { get; init; }

    /// <summary>The caller NGO's review stage of the record; null for callers outside an NGO (platform admins).</summary>
    public Vote.Monitor.Domain.Entities.ReviewAggregate.RecordReviewStage? ReviewStage { get; init; }

    public DateTime? StageChangedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }

    /// <summary>The caller NGO's members reviewing the record.</summary>
    public Vote.Monitor.Domain.Entities.ReviewAggregate.RecordReviewer[]? Reviewers { get; init; }

    /// <summary>The caller NGO's severity of the record (<c>NotAssessed</c> when it never set one).</summary>
    public Vote.Monitor.Domain.Entities.RecordSeverityAggregate.SeverityLevel? Severity { get; init; }

    public DateTime? SeverityChangedAt { get; init; }

    /// <summary>The caller NGO's labels on the record, sorted ignoring case.</summary>
    public string[]? Labels { get; init; }

    /// <summary>
    /// The report as platform admins see it (ADR 9): who sent it (name and email), the NGO, from where, the form, when,
    /// and its polling-station corrections. What the observer reported (answers, notes, media, counts, the location's
    /// description) and the NGO's follow-up are left out.
    /// </summary>
    public Response ForPlatformAdmin() => this with
    {
        FollowUpStatus = null,
        NumberOfQuestionsAnswered = null,
        NumberOfUnansweredQuestions = null,
        LocationDescription = null,
        Questions = null,
        Answers = null,
        Notes = null,
        Attachments = null
    };
}
