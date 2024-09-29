namespace Feature.IssueReports.Notes.List;

public record Response
{
    public required List<IssueReportNoteModel> Notes { get; init; }
}
