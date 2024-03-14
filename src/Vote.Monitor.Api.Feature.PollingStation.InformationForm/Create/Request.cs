namespace Vote.Monitor.Api.Feature.PollingStation.InformationForm.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public List<string> Languages { get; set; } = new();

}
