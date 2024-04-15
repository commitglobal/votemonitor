namespace Vote.Monitor.Api.Feature.Emergencies.ListReceived;

public record Response
{
    public required List<EmergencyModel> Attachments { get; init; }
}
