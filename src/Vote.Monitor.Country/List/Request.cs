using System.ComponentModel;
using FastEndpoints;

namespace Vote.Monitor.Country.List;

public class Request
{
    [QueryParam]
    public string? NameFilter { get; set; }

    [QueryParam]
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [QueryParam]
    [DefaultValue(100)]
    public int PageSize { get; set; } = 100;
}
