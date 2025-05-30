﻿namespace Feature.Notes;

public record NoteModel
{
    public required Guid Id { get; init; }
    public required Guid ElectionRoundId { get; init; }
    public required Guid PollingStationId { get; init; }
    public required Guid FormId { get; init; }
    public required Guid QuestionId { get; init; }
    public required string Text { get; init; }
    public required DateTime LastUpdatedAt { get; init; }

    public static NoteModel FromEntity(NoteAggregate note)
        => new ()
        {
            Id = note.Id,
            ElectionRoundId = note.ElectionRoundId,
            PollingStationId = note.PollingStationId,
            FormId = note.FormId,
            QuestionId = note.QuestionId,
            Text = note.Text,
            LastUpdatedAt = note.LastUpdatedAt
        };
}
