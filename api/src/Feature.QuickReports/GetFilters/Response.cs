namespace Feature.QuickReports.GetFilters;

public record Response
{
    public SubmissionsTimestampsFilterOptions TimestampsFilterOptions { get; init; }
}

public record SubmissionsTimestampsFilterOptions
{
    public DateTime? FirstSubmissionTimestamp { get; init; }
    public DateTime? LastSubmissionTimestamp { get; init; }
}