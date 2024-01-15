namespace Vote.Monitor.Core.Models;

public class BaseFilterRequest
{
    [QueryParam]
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [QueryParam]
    [DefaultValue(100)]
    public int PageSize { get; set; } = 100;

    [QueryParam]
    public string? ColumnName { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<SortOrder, string>))]
    public SortOrder? SortOrder { get; set; }
}
