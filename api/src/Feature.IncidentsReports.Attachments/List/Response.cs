namespace Feature.IncidentsReports.Attachments.List;

public record Response
{
    public IncidentReportAttachmentModel[] Attachments { get; init; } = [];
}