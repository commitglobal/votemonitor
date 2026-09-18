using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Feature.QuickReports.GetFilters;

public record Response
{
    public SubmissionsTimestampsFilterOptions TimestampsFilterOptions { get; init; }
    public List<SubmissionsObserverFilterOption> ObserverFilterOptions { get; init; } = [];
}

public record SubmissionsObserverFilterOption
{
    public Guid MonitoringObserverId { get; init; }
    public string DisplayName { get; init; } = null!;
    public string Email { get; init; } = null!;

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public UserStatus AccountStatus { get; init; } = null!;
}

public record SubmissionsTimestampsFilterOptions
{
    public DateTime? FirstSubmissionTimestamp { get; init; }
    public DateTime? LastSubmissionTimestamp { get; init; }
}
