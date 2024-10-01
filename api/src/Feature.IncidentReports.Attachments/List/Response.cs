namespace Feature.IncidentReports.Attachments.List;

public record Response
{
    public IncidentReportAttachmentModel[] Attachments { get; init; } = [];
}