namespace Feature.Notes;

public record NoteModelV2
{
    public required Guid Id { get; init; }
    public required Guid SubmissionId { get; init; }
    public required Guid QuestionId { get; init; }
    public required string Text { get; init; }
    public required DateTime LastUpdatedAt { get; init; }

    public static NoteModelV2 FromEntity(NoteAggregate note)
        => new ()
        {
            Id = note.Id,
            SubmissionId = note.SubmissionId,
            QuestionId = note.QuestionId,
            Text = note.Text,
            LastUpdatedAt = note.LastUpdatedAt,
        };
}
