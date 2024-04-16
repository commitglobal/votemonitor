using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Form.Submissions.ListEntries;

public record FormSubmissionEntries
{
    public string FormCode { get; init; }
    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public FormType FormType { get; init; } = default!;
    public Guid PollingStationId { get; init; }
    public Guid SubmissionId { get; init; }
    public string PollingStationLevel1 { get; init; } = default!;
    public string PollingStationLevel2 { get; init; } = default!;
    public string PollingStationLevel3 { get; init; } = default!;
    public string PollingStationLevel4 { get; init; } = default!;
    public string PollingStationLevel5 { get; init; } = default!;
    public string PollingStationNumber { get; init; } = default!;
    public DateTime SubmittedAt { get; init; }
    public Guid MonitoringObserverId { get; init; }
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public int NumberOfQuestionAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }
}
