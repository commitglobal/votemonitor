namespace Feature.IncidentsReports.Notes;

public record IncidentReportNoteModel
{
    public required Guid Id { get; init; }
    public required Guid ElectionRoundId { get; init; }
    public required Guid IncidentReportId { get; init; }
    public required Guid FormId { get; init; }
    public required Guid QuestionId { get; init; }
    public required string Text { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }

    public static IncidentReportNoteModel FromEntity(IncidentReportNoteAggregate note)
        => new()
        {
            Id = note.Id,
            ElectionRoundId = note.ElectionRoundId,
            IncidentReportId = note.IncidentReportId,
            FormId = note.FormId,
            QuestionId = note.QuestionId,
            Text = note.Text,
            CreatedAt = note.CreatedOn,
            UpdatedAt = note.LastModifiedOn
        };
}