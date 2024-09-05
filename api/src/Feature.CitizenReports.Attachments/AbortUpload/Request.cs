namespace Feature.CitizenReports.Attachments.AbortUpload;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid CitizenReportId { get; set; }
    public Guid Id { get; set; }
    public string UploadId { get; set; }
}
