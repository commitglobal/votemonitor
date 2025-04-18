namespace Feature.PollingStations.GetPollingStationsVersion;

public class Response
{
    public Guid ElectionRoundId { get; set; }
    public string CacheKey { get; set; }
}
