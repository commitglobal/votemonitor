namespace Vote.Monitor.Feature.PollingStation.GetAllPollingStations;
internal class PaginationResponse<T>
{
    public List<T> Results { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPges { get; set; }
}
