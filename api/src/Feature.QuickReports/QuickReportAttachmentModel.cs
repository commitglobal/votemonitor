namespace Feature.QuickReports;

public class QuickReportAttachmentModel
{
    public Guid Id { get; set; }
    public Guid QuickReportId { get; set; }
    public Guid ElectionRoundId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string PresignedUrl { get; set; } = string.Empty;
    public int UrlValidityInSeconds { get; set; }
}
