namespace Feature.ObserverGuide;

public record ObserverGuideModel
{
    public required Guid Id { get; init; }
    public required string FileName { get; init; } = string.Empty;
    public required string Title { get; init; } = string.Empty;
    public required string MimeType { get; init; } = string.Empty;
    public required string PresignedUrl { get; init; } = string.Empty;
    public required int UrlValidityInSeconds { get; init; }
}
