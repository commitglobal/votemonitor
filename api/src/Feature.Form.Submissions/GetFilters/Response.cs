using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Feature.Form.Submissions.Models;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Form.Module.Models;

namespace Feature.Form.Submissions.GetFilters;

public record Response
{
    public SubmissionsTimestampsFilterOptions TimestampsFilterOptions { get; init; }
    public List<SubmissionsFormFilterOption> FormFilterOptions { get; init; } = [];
}

public record SubmissionsFormFilterOption
{
    public Guid FormId { get; init; } = default!;
    public string FormCode { get; init; } = default!;
    public string FormName { get; init; } = default!;
}

public record SubmissionsTimestampsFilterOptions
{
    public DateTime? FirstSubmissionTimestamp { get; init; }
    public DateTime? LastSubmissionTimestamp { get; init; }
}