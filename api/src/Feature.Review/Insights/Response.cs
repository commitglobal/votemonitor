namespace Feature.Review.Insights;

/// <summary>
/// The chart's numbers: the total, one bar per value of the X axis (with its parts when grouped) and, when grouped,
/// the groups' own totals. Totals come from the records themselves, so distinct counts (observers, polling stations)
/// and records with several reviewers or tags are never counted twice in a total.
/// </summary>
public record InsightsModel(
    string XAxis,
    string? GroupBy,
    string Metric,
    long Total,
    InsightBarModel[] Bars,
    InsightBucketModel[]? Groups);

/// <summary>One value of an axis. Key null: the records without one (no reviewer, no place at that level…).</summary>
public record InsightBucketModel(string? Key, string? Label, long Value);

public record InsightBarModel(string? Key, string? Label, long Value, InsightBucketModel[]? Parts);
