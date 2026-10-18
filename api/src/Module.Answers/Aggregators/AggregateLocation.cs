namespace Module.Answers.Aggregators;

/// <summary>
/// A Level1..Level5 path of the aggregated submissions, computed by "AggregateAnswersByLocation".
/// The question aggregates hold their breakdown per location in <see cref="BaseAnswerAggregate.LocationAggregates"/>.
/// </summary>
public class AggregateLocation
{
    public int Id { get; init; }

    /// <summary>
    /// 1..5: the deepest level of the path.
    /// </summary>
    public int Level { get; init; }

    /// <summary>
    /// The location one level up with the same path; not set for Level1 locations.
    /// </summary>
    public int? ParentId { get; init; }

    public string? Level1 { get; init; }
    public string? Level2 { get; init; }
    public string? Level3 { get; init; }
    public string? Level4 { get; init; }
    public string? Level5 { get; init; }

    public int SubmissionCount { get; init; }
    public int TotalNumberOfQuestionsAnswered { get; init; }
    public int TotalNumberOfFlaggedAnswers { get; init; }

    /// <summary>
    /// Number of distinct polling stations of the location's submissions. Not set for citizen reports.
    /// </summary>
    public int? PollingStationsCount { get; init; }

    /// <summary>
    /// Number of distinct citizen report locations of the location's submissions. Only set for citizen reports.
    /// </summary>
    public int? LocationsCount { get; init; }
}
