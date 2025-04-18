namespace Feature.PollingStations.GetTagValues;
public class Request
{
    public required Guid ElectionRoundId { get; set; }
    public required string SelectTag { get; set; }

    public Dictionary<string, string>? Filter { get; set; }
}
