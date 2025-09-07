namespace Feature.Attachments;

public record AttachmentModelV2
{
    public required Guid Id { get; init; }
    public required Guid ElectionRoundId { get; init; }
    public Guid SubmissionId { get; init; }
    public required Guid QuestionId { get; init; }
    public required string FileName { get; init; } = string.Empty;
    public required string MimeType { get; init; } = string.Empty;
    public required string PresignedUrl { get; init; } = string.Empty;
    public required int UrlValidityInSeconds { get; init; }
}
