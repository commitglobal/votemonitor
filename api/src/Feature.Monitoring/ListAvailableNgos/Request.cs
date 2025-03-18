using Vote.Monitor.Core.Models;

namespace Feature.Monitoring.ListAvailableNgos;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }
    [QueryParam] public string? SearchText { get; set; }
}
