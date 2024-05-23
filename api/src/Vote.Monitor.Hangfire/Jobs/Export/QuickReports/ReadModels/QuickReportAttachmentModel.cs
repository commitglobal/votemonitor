namespace Vote.Monitor.Hangfire.Jobs.Export.QuickReports.ReadModels;

public class QuickReportAttachmentModel
{
    public Guid QuickReportId { get; set; }
    public Guid MonitoringObserverId { get; set; }
    public string FilePath { get; set; }
    public string UploadedFileName { get; set; }
    public string FileName { get; set; }
    public string MimeType { get; set; }
    public string PresignedUrl { get; set; }
    public int UrlValidityInSeconds { get; set; }

    public DateTime TimeSubmitted { get; init; }
}
