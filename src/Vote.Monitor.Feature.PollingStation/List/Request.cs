using FastEndpoints;
using System.ComponentModel;

namespace Vote.Monitor.Feature.PollingStation.List;
public class Request
{
    [QueryParam]
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [QueryParam]
    [DefaultValue(100)]
    public int PageSize { get; set; } = 100;

    [QueryParam]
    public string? AddressFilter { get; set; }

    [QueryParam]
    public Dictionary<string, string>? Filter { get; set; }
}
