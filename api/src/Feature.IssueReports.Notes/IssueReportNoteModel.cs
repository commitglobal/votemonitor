namespace Feature.IssueReports.Notes;

public record IssueReportNoteModel
{
    public required Guid Id { get; init; }
    public required Guid ElectionRoundId { get; init; }
    public required Guid IssueReportId { get; init; }
    public required Guid FormId { get; init; }
    public required Guid QuestionId { get; init; }
    public required string Text { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }

    public static IssueReportNoteModel FromEntity(IssueReportNoteAggregate note)
        => new()
        {
            Id = note.Id,
            ElectionRoundId = note.ElectionRoundId,
            IssueReportId = note.IssueReportId,
            FormId = note.FormId,
            QuestionId = note.QuestionId,
            Text = note.Text,
            CreatedAt = note.CreatedOn,
            UpdatedAt = note.LastModifiedOn
        };
}