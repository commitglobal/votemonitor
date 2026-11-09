using Vote.Monitor.Core.RulesEngine.Rules;

namespace Feature.Review.Insights;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public ReportRecordType RecordType { get; set; } = null!;

    /// <summary>Ignored for citizen reports, which have no coalition view.</summary>
    public DataSource DataSource { get; set; } = DataSource.Ngo;

    /// <summary>The same filter as the report list, so the numbers match it.</summary>
    public FilterRule? Filter { get; set; }

    /// <summary>What each bar is (an axis key, see <see cref="InsightAxes"/>).</summary>
    public string XAxis { get; set; } = null!;

    /// <summary>Optionally splits each bar by a second axis.</summary>
    public string? GroupBy { get; set; }

    /// <summary>What the bars measure (see <see cref="InsightMetrics"/>); records by default.</summary>
    public string Metric { get; set; } = InsightMetrics.Records;

    /// <summary>The IANA time zone the day and hour axes are counted in (the user's display setting).</summary>
    public string TimeZone { get; set; } = "UTC";
}
