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

    public SubmissionFollowUpStatus FollowUpStatus { get; init; }
    public MonitoringObserverStatus MonitoringObserverStatus { get; init; }

    public bool IsCompleted { get; set; }
}
