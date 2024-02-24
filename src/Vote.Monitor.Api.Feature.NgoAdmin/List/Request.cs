namespace Vote.Monitor.Api.Feature.NgoAdmin.List;

public class Request : BaseSortPaginatedRequest
{
    [QueryParam]
    public string? NameFilter { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public UserStatus? Status { get; set; }
}
