namespace Feature.IssueReports.Attachments.List;

public record Response
{
    public IssueReportAttachmentModel[] Attachments { get; init; } = [];
}