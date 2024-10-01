namespace Feature.IncidentsReports.Attachments.Get;

public class Request
{
    public  Guid ElectionRoundId { get; set; }

    public  Guid IncidentReportId { get; set; }
    public  Guid Id { get; set; }
}
