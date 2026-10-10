namespace Vote.Monitor.Core.Models;

public class BasePaginatedBodyRequest
{
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [DefaultValue(25)]
    public int PageSize { get; set; } = 25;
}
