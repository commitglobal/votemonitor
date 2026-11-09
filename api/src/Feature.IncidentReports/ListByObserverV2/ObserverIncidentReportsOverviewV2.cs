using System.Text.Json;

namespace Feature.IncidentReports.ListByObserverV2;

/// <summary>How many of the observer's incident reports are on one of the caller NGO's follow-up statuses.</summary>
public record FollowUpCount(Guid Id, string Name, string? Color, string? Icon, int Count);

public record ObserverIncidentReportsOverviewV2
{
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public string[] Tags { get; init; } = [];
    public int NumberOfFlaggedAnswers { get; init; }
    public int NumberOfIncidentsSubmitted { get; init; }

    /// <summary>The observer's (matching) incident reports per follow-up status of the caller's NGO, in the statuses' order.</summary>
    public FollowUpCount[] FollowUps { get; init; } = [];
}

/// <summary>The row as the query returns it: the follow-up counts as JSON.</summary>
internal record ObserverIncidentReportsRow
{
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public string[] Tags { get; init; } = [];
    public int NumberOfFlaggedAnswers { get; init; }
    public int NumberOfIncidentsSubmitted { get; init; }
    public string? FollowUpsJson { get; init; }

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public ObserverIncidentReportsOverviewV2 ToModel() => new()
    {
        MonitoringObserverId = MonitoringObserverId,
        ObserverName = ObserverName,
        Email = Email,
        PhoneNumber = PhoneNumber,
        Tags = Tags,
        NumberOfFlaggedAnswers = NumberOfFlaggedAnswers,
        NumberOfIncidentsSubmitted = NumberOfIncidentsSubmitted,
        FollowUps = FollowUpsJson is null ? [] : JsonSerializer.Deserialize<FollowUpCount[]>(FollowUpsJson, Json) ?? []
    };
}
