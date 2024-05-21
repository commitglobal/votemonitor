using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers.List;
public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [QueryParam]
    public string[]? Tags { get; set; } = [];

    [QueryParam]
    public string? SearchText { get; set; }

    [QueryParam]
    public MonitoringObserverStatus? Status { get; set; }
}
