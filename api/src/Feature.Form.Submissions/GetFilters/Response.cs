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