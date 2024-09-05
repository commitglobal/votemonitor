namespace Feature.CitizenReports.Notes.List;

public record Response
{
    public required List<CitizenReportNoteModel> Notes { get; init; }
}
