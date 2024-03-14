namespace Vote.Monitor.Api.Feature.PollingStation.Notes;

public record NoteModel
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }
}
