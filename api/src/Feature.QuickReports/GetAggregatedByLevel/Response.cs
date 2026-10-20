using Vote.Monitor.Core.Models;

namespace Feature.QuickReports.GetAggregatedByLevel;

public class Response
{
    public QuickReportsAggregateByLevel QuickReportsAggregate { get; set; } = null!;
}

/// <summary>
/// Quick reports aggregated over all the matching quick reports and per location of one level.
/// <see cref="QuickReportsAggregate.Locations"/> holds the distinct Level1..<see cref="Level"/> paths; quick reports without a
/// value at <see cref="Level"/> (such as the ones not related to a polling station) are left out of them.
/// </summary>
public class QuickReportsAggregateByLevel : QuickReportsAggregate
{
    /// <summary>
    /// The level the quick reports are broken down by.
    /// </summary>
    public LocationLevel Level { get; set; } = null!;
}
