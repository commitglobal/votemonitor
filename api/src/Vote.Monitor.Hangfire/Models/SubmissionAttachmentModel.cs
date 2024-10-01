namespace Vote.Monitor.Hangfire.Models;

public class SubmissionAttachmentModel
{
    public Guid QuestionId { get; set; }
    public string FilePath { get; set; }
    public string UploadedFileName { get; set; }
    public string PresignedUrl { get; set; }
}