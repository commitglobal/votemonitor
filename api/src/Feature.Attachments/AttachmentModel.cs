namespace Feature.Attachments;

[Obsolete("Will be removed in future version")]

public record AttachmentModel
{
    public required Guid Id { get; init; }
    public Guid ElectionRoundId { get; set; }
    public required Guid PollingStationId { get; init; }
    public required Guid FormId { get; init; }
    public required Guid QuestionId { get; init; }
    public required string FileName { get; init; } = string.Empty;
    public required string MimeType { get; init; } = string.Empty;
    public required string PresignedUrl { get; init; } = string.Empty;
    public required int UrlValidityInSeconds { get; init; }
}
