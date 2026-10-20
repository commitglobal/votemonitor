using Vote.Monitor.Core.Models;

namespace Feature.QuickReports;

/// <summary>
/// Quick reports aggregated over all the matching quick reports and per location (see <see cref="QuickReportsAggregateQuery"/>).
/// </summary>
public class QuickReportsAggregate : QuickReportsCounts
{
    /// <summary>
    /// The Level1..Level5 paths of the quick reports' polling stations, each aggregating its quick reports.
    /// </summary>
    public QuickReportsLocationAggregate[] Locations { get; init; } = [];
}

/// <summary>
/// The quick reports of one Level1..Level5 path.
/// </summary>
public class QuickReportsLocationAggregate : QuickReportsCounts
{
    public int Id { get; init; }

    /// <summary>
    /// The deepest level of the path.
    /// </summary>
    public LocationLevel Level { get; init; } = null!;

    /// <summary>
    /// The location one level up with the same path, when that level is part of the aggregate; not set for Level1 locations.
    /// </summary>
    public int? ParentId { get; init; }

    public string? Level1 { get; init; }
    public string? Level2 { get; init; }
    public string? Level3 { get; init; }
    public string? Level4 { get; init; }
    public string? Level5 { get; init; }
}

/// <summary>
/// The counts of a set of quick reports. The histograms hold every incident category, location type and follow-up status,
/// with 0 for the ones not reported.
/// </summary>
public abstract class QuickReportsCounts
{
    public int QuickReportCount { get; init; }
    public int PollingStationsCount { get; init; }
    public int ObserversCount { get; init; }

    /// <summary>
    /// Number of quick reports per incident category.
    /// </summary>
    public IReadOnlyDictionary<string, int> IncidentCategoryHistogram { get; init; } = new Dictionary<string, int>();

    /// <summary>
    /// Number of quick reports per location type.
    /// </summary>
    public IReadOnlyDictionary<string, int> LocationTypeHistogram { get; init; } = new Dictionary<string, int>();

    /// <summary>
    /// Number of quick reports per follow-up status.
    /// </summary>
    public IReadOnlyDictionary<string, int> FollowUpStatusHistogram { get; init; } = new Dictionary<string, int>();
}
