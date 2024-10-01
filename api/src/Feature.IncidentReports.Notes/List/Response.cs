namespace Feature.IncidentReports.Notes.List;

public record Response
{
    public required List<IncidentReportNoteModel> Notes { get; init; }
}
