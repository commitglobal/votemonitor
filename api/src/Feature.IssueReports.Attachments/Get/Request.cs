namespace Feature.IssueReports.Attachments.Get;

public class Request
{
    public  Guid ElectionRoundId { get; set; }

    public  Guid IssueReportId { get; set; }
    public  Guid Id { get; set; }
}
