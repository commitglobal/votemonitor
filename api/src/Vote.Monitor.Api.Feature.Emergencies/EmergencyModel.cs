namespace Vote.Monitor.Api.Feature.Emergencies;

public record EmergencyModel
{
    public required Guid Id { get; init; }
    public required Guid ObserverId { get; init; }
    public required string ObserverName { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }

    public List<AttachmentModel> Attachments { get; set; } = new();
}

public class AttachmentModel
{
    public required Guid Id { get; set; }
    public required string FileName { get; init; } = string.Empty;
    public required string MimeType { get; init; } = string.Empty;
    public required string PresignedUrl { get; init; } = string.Empty;
    public required int UrlValidityInSeconds { get; init; }

}
