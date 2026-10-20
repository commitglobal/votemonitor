using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.QuickReports.GetAggregatedByLevel;

public class Request : BaseFilterConditionsRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    /// <summary>
    /// The level to break the quick reports down by.
    /// </summary>
    public LocationLevel Level { get; set; } = null!;

    public DataSource DataSource { get; set; } = DataSource.Ngo;
}
