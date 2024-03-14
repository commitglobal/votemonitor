namespace Vote.Monitor.Api.Feature.PollingStation.InformationForm.Get;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid Id { get; set; }
}
