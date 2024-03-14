namespace Vote.Monitor.Api.Feature.Answers.Attachments.List;

public record Response
{
    public required List<AttachmentModel> Attachments { get; init; }
}
