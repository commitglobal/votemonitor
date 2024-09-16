using Feature.MonitoringObservers.Parser;
using Vote.Monitor.Core.Security;

namespace Feature.MonitoringObservers.Add;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public MonitoringObserverImportModel[] Observers { get; set; } = [];
}