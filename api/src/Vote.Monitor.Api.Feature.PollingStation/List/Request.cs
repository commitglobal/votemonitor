using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.List;
public class Request: BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [QueryParam]
    public string? AddressFilter { get; set; }
}
