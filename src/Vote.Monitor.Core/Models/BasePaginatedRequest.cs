namespace Vote.Monitor.Core.Models;

public class BasePaginatedRequest
{
    [QueryParam]
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [QueryParam]
    [DefaultValue(25)]
    public int PageSize { get; set; } = 25;
}
