using Vote.Monitor.Domain.Entities.ReviewAggregate;
using Module.Forms.Models;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Module.Answers.Models;

public record FormSubmissionViewV2
{
    public Guid SubmissionId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid FormId { get; init; }
    public string FormCode { get; init; } = null!;
    public string DefaultLanguage { get; init; } = null!;
    public string[] Languages { get; init; } = [];
    public FormType FormType { get; init; } = null!;
    public TranslatedString FormName { get; init; } = null!;

    /// <summary>Null for platform admins, like everything else the NGO keeps about the record.</summary>
    public SubmissionFollowUpStatus? FollowUpStatus { get; init; }

    public Guid PollingStationId { get; init; }
    public string Level1 { get; init; } = null!;
    public string Level2 { get; init; } = null!;
    public string Level3 { get; init; } = null!;
    public string Level4 { get; init; } = null!;
    public string Level5 { get; init; } = null!;
    public string Number { get; init; } = null!;
    public string? Address { get; init; }
    public Guid MonitoringObserverId { get; init; }
    public bool IsOwnObserver { get; init; }
    public string ObserverName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public string[]? Tags { get; init; } = [];
    public string NgoName { get; init; } = null!;
    public int? NumberOfFlaggedAnswers { get; init; }
    /// <summary>The questions the observer was shown (display logic applied) that they answered.</summary>
    public int? NumberOfQuestionsAnswered { get; init; }

    /// <summary>The questions the observer was shown (display logic applied) and left unanswered.</summary>
    public int? NumberOfUnansweredQuestions { get; init; }

    public BaseQuestionModel[]? Questions { get; init; }
    public BaseAnswerModel[]? Answers { get; init; } = [];
    public NoteModel[]? Notes { get; init; } = [];
    public AttachmentModel[]? Attachments { get; init; } = [];

    public DateTime? ArrivalTime { get; init; }
    public DateTime? DepartureTime { get; init; }
    public ObservationBreakModel[]? Breaks { get; init; } = [];
    public bool IsCompleted { get; init; }

    /// <summary>Moves of this submission to another polling station by a platform admin, oldest first.</summary>
    public PollingStationCorrectionModel[] PollingStationCorrections { get; init; } = [];

    /// <summary>
    /// Which of the observer's submissions of this form at this polling station this is (by creation), and how many there are;
    /// 1 of 1 for polling station information.
    /// </summary>
    public int SubmissionNumber { get; init; } = 1;
    public int SubmissionCount { get; init; } = 1;

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
    /// The submission as platform admins see it (ADR 9): who sent it (name and email), from where, the form, when, and
    /// its polling-station corrections. What the observer reported (answers, notes, media, counts, arrival and
    /// departure) and what the NGO keeps about the record or the observer (phone, tags, follow-up) are left out.
    /// </summary>
    public FormSubmissionViewV2 ForPlatformAdmin() => this with
    {
        FollowUpStatus = null,
        PhoneNumber = null,
        Tags = null,
        NumberOfFlaggedAnswers = null,
        NumberOfQuestionsAnswered = null,
        NumberOfUnansweredQuestions = null,
        Questions = null,
        Answers = null,
        Notes = null,
        Attachments = null,
        ArrivalTime = null,
        DepartureTime = null,
        Breaks = null
    };
}
