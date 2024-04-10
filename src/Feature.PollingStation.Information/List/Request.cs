using Vote.Monitor.Core.Models;

namespace Feature.PollingStation.Information.List;

public class Request: BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim("NgoId")]
    public Guid NgoId { get; set; }
}
