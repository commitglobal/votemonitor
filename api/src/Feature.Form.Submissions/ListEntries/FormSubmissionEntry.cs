using Vote.Monitor.Domain.Entities.FormAggregate;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Form.Submissions.ListEntries;

public record FormSubmissionEntry
{
    public Guid SubmissionId { get; init; }
    public DateTime TimeSubmitted { get; init; }

    public Guid FormId { get; init; }
    public string FormCode { get; init; } = null!;
    public TranslatedString FormName { get; init; } = null!;

    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public FormType FormType { get; init; } = null!;

    [JsonConverter(typeof(SmartEnumNameConverter<FormStatus, string>))]
    public FormStatus FormStatus { get; init; } = null!;

    public string DefaultLanguage { get; set; }
    public Guid PollingStationId { get; init; }
    public string Level1 { get; init; } = null!;
    public string Level2 { get; init; } = null!;
    public string Level3 { get; init; } = null!;
    public string Level4 { get; init; } = null!;
    public string Level5 { get; init; } = null!;
    public string Number { get; init; } = null!;
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null!;
    public string NgoName { get; init; } = null!;
    public string[] Tags { get; init; } = [];
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }
    public int MediaFilesCount { get; init; }
    public int NotesCount { get; init; }
    public int CommentsCount { get; init; }
    public bool HasComments { get; init; }
    public bool HasNotes { get; init; }
    public bool HasAttachments { get; init; }

    public SubmissionFollowUpStatus FollowUpStatus { get; init; }

    /// <summary>The record's review stage in the caller's NGO.</summary>
    public Guid StageId { get; init; }
    public string StageName { get; init; } = "";
    public string ReviewStatus { get; init; } = "";
    public DateTime? StageChangedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }

    /// <summary>The record's reviewers in the caller's NGO.</summary>
    public Guid[] ReviewerIds { get; init; } = [];

    /// <summary>The record's severity in the caller's NGO: None, Low, Medium, High or Urgent.</summary>
    public string Severity { get; init; } = "None";
    public MonitoringObserverStatus MonitoringObserverStatus { get; init; }

    public bool IsCompleted { get; set; }
}
