namespace Vote.Monitor.Api.Feature.Emergencies.Attachments.List;

public record Response
{
    public required List<AttachmentModel> Attachments { get; init; }
}
