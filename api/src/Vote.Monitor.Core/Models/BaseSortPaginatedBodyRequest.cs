namespace Vote.Monitor.Core.Models;

public class BaseSortPaginatedBodyRequest : BasePaginatedBodyRequest
{
    public string? SortColumnName { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<SortOrder, string>))]
    public SortOrder? SortOrder { get; set; }

    [JsonIgnore]
    // We want the "asc" to be the default, that's why the condition is reverted.
    public bool IsAscendingSorting => !(SortOrder?.Equals(SortOrder.Desc) ?? false);
}
