namespace Feature.QuickReports;

public class QuickReportAttachmentModel
{
    public Guid Id { get; set; }
    public Guid QuickReportId { get; set; }
    public string FilePath { get; set; } = String.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string PresignedUrl { get; set; } = string.Empty;
    public int UrlValidityInSeconds { get; set; }
    public string UploadedFileName { get; set; } = string.Empty;

    public DateTime TimeSubmitted { get; set; }
}
