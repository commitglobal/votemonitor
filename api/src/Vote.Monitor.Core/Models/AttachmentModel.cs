namespace Vote.Monitor.Core.Models;

public record AttachmentModel
{
    public Guid SubmissionId { get; init; }
    public Guid QuestionId { get; init; }
    public Guid MonitoringObserverId { get; init; }
    public string FilePath { get; init; }
    public string UploadedFileName { get; init; }
    public string FileName { get; init; }
    public string MimeType { get; init; }
    public string PresignedUrl { get; init; }
    public int UrlValidityInSeconds { get; init; }

    public DateTime TimeSubmitted { get; init; }
}
