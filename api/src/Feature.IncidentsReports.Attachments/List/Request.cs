namespace Feature.IncidentsReports.Attachments.List;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid IncidentReportId { get; set; }
    public Guid FormId { get; set; }
}