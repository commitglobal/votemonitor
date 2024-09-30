namespace Feature.IncidentsReports.Notes.List;

public record Response
{
    public required List<IncidentReportNoteModel> Notes { get; init; }
}
