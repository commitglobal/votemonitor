namespace Feature.PollingStations.Import;

public class Request
{
    public required Guid ElectionRoundId { get; set; }
    public required IFormFile File { get; set; }
}
