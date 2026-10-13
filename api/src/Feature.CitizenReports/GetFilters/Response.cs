namespace Feature.CitizenReports.GetFilters;

using Vote.Monitor.Core.Models;

public record Response
{
    public SubmissionsTimestampsFilterOptions TimestampsFilterOptions { get; init; }
    public List<SubmissionsFormFilterOption> FormFilterOptions { get; init; } = [];
}

public record SubmissionsFormFilterOption
{
    public Guid FormId { get; init; } = Guid.Empty!;
    public string FormCode { get; init; } = null!;
    public TranslatedString FormName { get; init; } = null!;
    public string DefaultLanguage { get; init; } = null!;
}

public record SubmissionsTimestampsFilterOptions
{
    public DateTime? FirstSubmissionTimestamp { get; init; }
    public DateTime? LastSubmissionTimestamp { get; init; }
}