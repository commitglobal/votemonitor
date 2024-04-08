namespace Feature.MonitoringObservers.Import;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid MonitoringNgoId { get; set; }
    public IFormFile File { get; set; }
}
