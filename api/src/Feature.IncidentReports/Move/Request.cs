namespace Feature.IncidentReports.Move;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid IncidentReportId { get; set; }

    public Guid PollingStationId { get; set; }
}
