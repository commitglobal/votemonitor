using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers.List;
public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    public Guid MonitoringNgoId { get; set; }
    
    [QueryParam]
    public string[]? Tags { get; set; } = [];

    [QueryParam]
    public string? NameFilter { get; set; }

    [QueryParam]
    public string? EmailFilter { get; set; }

    [QueryParam]
    public MonitoringObserverStatus? Status { get; set; }
}
