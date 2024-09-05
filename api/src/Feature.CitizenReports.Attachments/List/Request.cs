namespace Feature.CitizenReports.Attachments.List;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid CitizenReportId { get; set; }
    public Guid FormId { get; set; }
}