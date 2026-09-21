using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.Statistics.GetMonitoringObserverLevelStatistics;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid MonitoringObserverId { get; set; }

    [QueryParam]
    public DataSource DataSource { get; set; } = DataSource.Ngo;
}
