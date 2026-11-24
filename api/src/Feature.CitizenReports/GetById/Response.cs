using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Module.Answers.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Module.Forms.Models;

namespace Feature.CitizenReports.GetById;

public class Response
{
    public Guid CitizenReportId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid FormId { get; init; }
    public string FormCode { get; init; }
    public TranslatedString FormName { get; init; }
    public string FormDefaultLanguage { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<CitizenReportFollowUpStatus, string>))]
    public CitizenReportFollowUpStatus FollowUpStatus { get; init; }

    /// <summary>The questions the citizen was shown (display logic applied) that they answered.</summary>
    public int NumberOfQuestionsAnswered { get; init; }

    /// <summary>The questions the citizen was shown (display logic applied) and left unanswered.</summary>
    public int NumberOfUnansweredQuestions { get; init; }

    public string LocationId { get; init; }
    public string LocationLevel1 { get; init; }
    public string LocationLevel2 { get; init; }
    public string LocationLevel3 { get; init; }
    public string LocationLevel4 { get; init; }
    public string LocationLevel5 { get; init; }

    public BaseQuestionModel[] Questions { get; init; } = [];
    public BaseAnswerModel[] Answers { get; init; } = [];
    public NoteModel[] Notes { get; init; } = [];
    public SubmissionAttachmentModel[] Attachments { get; init; } = [];

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
}
