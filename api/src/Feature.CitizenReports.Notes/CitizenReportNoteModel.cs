namespace Feature.CitizenReports.Notes;

public record CitizenReportNoteModel
{
    public required Guid Id { get; init; }
    public required Guid ElectionRoundId { get; init; }
    public required Guid CitizenReportId { get; init; }
    public required Guid FormId { get; init; }
    public required Guid QuestionId { get; init; }
    public required string Text { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }

    public static CitizenReportNoteModel FromEntity(CitizenReportNoteAggregate note)
        => new()
        {
            Id = note.Id,
            ElectionRoundId = note.ElectionRoundId,
            CitizenReportId = note.CitizenReportId,
            FormId = note.FormId,
            QuestionId = note.QuestionId,
            Text = note.Text,
            CreatedAt = note.CreatedOn,
            UpdatedAt = note.LastModifiedOn
        };
}