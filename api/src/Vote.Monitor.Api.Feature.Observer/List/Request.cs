namespace Vote.Monitor.Api.Feature.Observer.List;

public class Request: BaseSortPaginatedRequest
{
    [QueryParam]
    public string? SearchText { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public UserStatus? Status { get; set; }
}
