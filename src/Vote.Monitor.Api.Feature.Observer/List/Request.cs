using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.Observer.List;

public class Request: BaseFilterRequest
{
    [QueryParam]
    public string? NameFilter { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public UserStatus? Status { get; set; }
}
