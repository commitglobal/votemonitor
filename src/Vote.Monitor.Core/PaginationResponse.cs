namespace Vote.Monitor.Core;
public class PaginationResponse<T>
{ 
    public required List<T> Results { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPges { get; set; }
}
