namespace Feature.IncidentReports.Notes.List;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid IncidentReportId { get; set; }
}