namespace Vote.Monitor.Api.Feature.Emergencies.Attachments;

public record AttachmentModel
{
    public required Guid Id { get; init; }
    public required string FileName { get; init; } = string.Empty;
    public required string MimeType { get; init; } = string.Empty;
    public required string PresignedUrl { get; init; } = string.Empty;
    public required int UrlValidityInSeconds { get; init; }
}
