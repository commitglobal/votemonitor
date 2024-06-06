namespace Feature.Statistics.GetElectionsOverview;

public class Request
{
    [FromHeader(HeaderName = "x-vote-monitor-api-key")]
    public string ApiKey { get; set; }
    public Guid[] ElectionRoundIds { get; set; }
}
