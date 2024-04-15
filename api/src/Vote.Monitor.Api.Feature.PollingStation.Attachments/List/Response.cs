namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.List;

public record Response
{
    public required List<AttachmentModel> Attachments { get; init; }
}
