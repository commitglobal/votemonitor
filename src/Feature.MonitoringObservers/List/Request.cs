using Vote.Monitor.Core.Models;

namespace Feature.MonitoringObservers.List;
public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }
    public Guid MonitoringNgoId { get; set; }
    
    [QueryParam]
    public string[]? Tags { get; set; } = [];

    [QueryParam]
    public string? NameFilter { get; set; }
}
