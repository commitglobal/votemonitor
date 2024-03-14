using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.List;
public class Request: BaseSortPaginatedRequest
{
    [QueryParam]
    public string? AddressFilter { get; set; }

    public Dictionary<string, string>? Filter { get; set; }
}
