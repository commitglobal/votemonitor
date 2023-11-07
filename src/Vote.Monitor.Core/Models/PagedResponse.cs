using System.Text.Json.Serialization;

namespace Vote.Monitor.Core.Models;

public class PagedResponse<T>
{
    public int CurrentPage { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public List<T> Items { get; private set; }

    [JsonConstructor]
    public PagedResponse(List<T> items, int totalCount, int currentPage, int pageSize)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        Items = items;
    }
}
