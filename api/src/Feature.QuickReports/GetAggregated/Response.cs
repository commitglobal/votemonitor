namespace Feature.QuickReports.GetAggregated;

public class Response
{
    /// <summary>
    /// Locations holds every Level1..Level5 path of the quick reports' polling stations, linked to the level above by ParentId.
    /// </summary>
    public QuickReportsAggregate QuickReportsAggregate { get; set; } = null!;
}
