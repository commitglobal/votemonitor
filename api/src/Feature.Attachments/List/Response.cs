namespace Feature.Attachments.List;

public record Response
{
    public required List<AttachmentModel> Attachments { get; init; }
}
