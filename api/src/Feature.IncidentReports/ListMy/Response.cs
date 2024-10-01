namespace Feature.IncidentReports.ListMy;

public record Response
{
    public required List<IncidentReportModel> IncidentReports { get; init; }
}