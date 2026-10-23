namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>
/// Filters on the caller NGO's workflow columns, for queries that join <c>"GetRecordReviewWorkflow"</c> (the V1
/// lists) or read the <c>Get*Entries</c> functions, which return the same columns.
/// </summary>
public static class ReviewSql
{
    /// <summary>
    /// Conditions (each starting with <c>AND</c>) for the simple workflow filters. Parameters: <c>@stageId</c>,
    /// <c>@reviewStatus</c>, <c>@reviewerId</c>, <c>@hasReviewer</c>; a null parameter doesn't filter.
    /// </summary>
    public static string Filters(string alias = "WF") => $"""
        AND (@stageId IS NULL OR {alias}."StageId" = @stageId)
        AND (@reviewStatus IS NULL OR {alias}."ReviewStatus" = @reviewStatus)
        AND (@reviewerId IS NULL OR @reviewerId = ANY({alias}."ReviewerIds"))
        AND (@hasReviewer IS NULL OR (cardinality({alias}."ReviewerIds") > 0) = @hasReviewer)
        """;
}
