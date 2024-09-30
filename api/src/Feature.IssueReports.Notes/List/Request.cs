namespace Feature.IssueReports.Notes.List;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid IssueReportId { get; set; }
}