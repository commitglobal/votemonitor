namespace Feature.PollingStation.Information.Move;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid Id { get; set; }

    public Guid PollingStationId { get; set; }
}
