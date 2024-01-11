namespace Vote.Monitor.Api.Feature.PollingStation.List;
public class Request: BaseFilterRequest
{
    [QueryParam]
    public string? AddressFilter { get; set; }

    [QueryParam]
    public Dictionary<string, string>? Filter { get; set; }
}
