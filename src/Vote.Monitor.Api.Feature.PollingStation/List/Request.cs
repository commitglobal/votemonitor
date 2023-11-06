using System.ComponentModel;

namespace Vote.Monitor.Api.Feature.PollingStation.List;
public class Request
{
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [DefaultValue(100)]
    public int PageSize { get; set; } = 100;

    [QueryParam]
    public string? AddressFilter { get; set; }

    [QueryParam]
    public Dictionary<string, string>? Filter { get; set; }
}
