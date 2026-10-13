namespace Vote.Monitor.Core.Models;

public record SubmissionAttachmentModel : AttachmentModel
{
    public Guid SubmissionId { get; init; }
}
