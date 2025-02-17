using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Form.Submissions.ListEntries;

public record FormSubmissionEntry
{
    public Guid SubmissionId { get; init; }
    public DateTime TimeSubmitted { get; init; }

    public string FormCode { get; init; } = default!;
    public TranslatedString FormName { get; init; } = default!;

    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public FormType FormType { get; init; } = default!;

    public string DefaultLanguage { get; set; }
    public Guid PollingStationId { get; init; }
    public string Level1 { get; init; } = default!;
    public string Level2 { get; init; } = default!;
    public string Level3 { get; init; } = default!;
    public string Level4 { get; init; } = default!;
    public string Level5 { get; init; } = default!;
    public string Number { get; init; } = default!;
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? PhoneNumber { get; init; } = default!;
    public string NgoName { get; init; } = default!;
    public string[] Tags { get; init; } = [];
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }
    public int MediaFilesCount { get; init; }
    public int NotesCount { get; init; }

    public SubmissionFollowUpStatus FollowUpStatus { get; init; }
    public MonitoringObserverStatus MonitoringObserverStatus { get; init; }

    public bool IsCompleted { get; set; }
}
