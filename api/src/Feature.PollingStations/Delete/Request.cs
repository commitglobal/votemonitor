namespace Feature.PollingStations.Delete;

public class Request
{
    public required Guid ElectionRoundId { get; set; }
    public required Guid Id { get; set; }
}
