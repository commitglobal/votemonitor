using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;

namespace Feature.Form.Submissions.ListByObserver;

public record ObserverSubmissionOverview
{
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string PhoneNumber { get; init; } = default!;
    public string NgoName { get; init; } = default!;
    public string[] Tags { get; init; } = [];
    public int NumberOfFlaggedAnswers { get; init; }
    public int NumberOfLocations { get; init; }
    public int NumberOfFormsSubmitted { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<SubmissionFollowUpStatus, string>))]
    public SubmissionFollowUpStatus? FollowUpStatus { get; init; }

    public bool IsOwnObserver { get; set; }
}
