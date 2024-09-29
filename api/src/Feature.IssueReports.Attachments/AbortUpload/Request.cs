namespace Feature.IssueReports.Attachments.AbortUpload;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid IssueReportId { get; set; }
    public Guid Id { get; set; }
    public string UploadId { get; set; }
}
