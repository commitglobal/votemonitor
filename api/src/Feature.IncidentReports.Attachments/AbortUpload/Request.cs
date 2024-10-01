namespace Feature.IncidentReports.Attachments.AbortUpload;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid IncidentReportId { get; set; }
    public Guid Id { get; set; }
    public string UploadId { get; set; }
}
