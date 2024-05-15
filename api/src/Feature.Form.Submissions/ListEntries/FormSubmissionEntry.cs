using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Form.Submissions.ListEntries;

public record FormSubmissionEntry
{
    public Guid SubmissionId { get; init; }
    public DateTime TimeSubmitted { get; init; }

    public string FormCode { get; init; } = default!;

    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public FormType FormType { get; init; } = default!;

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
    public string PhoneNumber { get; init; } = default!;
    public string[] Tags { get; init; }
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }
    public int MediaFilesCount { get; init; }
    public int NotesCount { get; init; }
    public bool? NeedsFollowUp { get; init; }
}
