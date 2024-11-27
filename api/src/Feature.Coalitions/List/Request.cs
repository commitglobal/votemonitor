using Vote.Monitor.Core.Models;

namespace Feature.NgoCoalitions.List;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }
    [QueryParam] public string? SearchText { get; set; }
}
