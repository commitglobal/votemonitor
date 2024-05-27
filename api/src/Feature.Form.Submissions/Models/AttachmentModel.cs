namespace Feature.Form.Submissions.Models;

public class AttachmentModel
{
    public Guid QuestionId { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid MonitoringObserverId { get; set; }
    public string FilePath { get; set; }
    public string UploadedFileName { get; set; }
    public string FileName { get; set; }
    public string MimeType { get; set; }
    public string PresignedUrl { get; set; }
    public int UrlValidityInSeconds { get; set; }

    public DateTime TimeSubmitted { get; init; }
}
