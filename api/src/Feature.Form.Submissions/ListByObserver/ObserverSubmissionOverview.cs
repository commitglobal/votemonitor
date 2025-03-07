using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;

namespace Feature.Form.Submissions.ListByObserver;

public record ObserverSubmissionOverview
{
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null!;
    public string NgoName { get; init; } = null!;
    public string[] Tags { get; init; } = [];
    public int NumberOfFlaggedAnswers { get; init; }
    public int NumberOfLocations { get; init; }
    public int NumberOfFormsSubmitted { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<SubmissionFollowUpStatus, string>))]
    public SubmissionFollowUpStatus? FollowUpStatus { get; init; }

    public bool IsOwnObserver { get; set; }
}
