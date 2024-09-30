namespace Feature.CitizenReports.Attachments.List;

public record Response
{
    public CitizenReportAttachmentModel[] Attachments { get; init; } = [];
}