namespace Vote.Monitor.Api.Feature.Emergencies.ListSubmitted;

public record Response
{
    public required List<EmergencyModel> Attachments { get; init; }
}
