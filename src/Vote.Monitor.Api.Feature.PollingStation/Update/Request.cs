namespace Vote.Monitor.Api.Feature.PollingStation.Update;
public class Request
{
    public required Guid ElectionRoundId { get; set; }
    public required Guid Id { get; set; }
    public required int DisplayOrder { get; set; }
    public required string Address { get; set; }
    public required Dictionary<string, string> Tags { get; set; }
}
