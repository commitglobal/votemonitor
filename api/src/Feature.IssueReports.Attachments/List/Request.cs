namespace Feature.IssueReports.Attachments.List;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid IssueReportId { get; set; }
    public Guid FormId { get; set; }
}