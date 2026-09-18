using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Form.Submissions.GetFilters;

public record Response
{
    public SubmissionsTimestampsFilterOptions TimestampsFilterOptions { get; init; }
    public List<SubmissionsFormFilterOption> FormFilterOptions { get; init; } = [];
    public List<SubmissionsObserverFilterOption> ObserverFilterOptions { get; init; } = [];
}

public record SubmissionsFormFilterOption
{
    public Guid FormId { get; init; } = Guid.Empty!;
    public string FormCode { get; init; } = null!;
    public TranslatedString FormName { get; init; } = null!;
    public string DefaultLanguage { get; init; } = null!;
    public string[] Languages { get; init; } = [];

    [JsonConverter(typeof(SmartEnumNameConverter<FormStatus, string>))]
    public FormStatus FormStatus { get; init; } = null!;
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
