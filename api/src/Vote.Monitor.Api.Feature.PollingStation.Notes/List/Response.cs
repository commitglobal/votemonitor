namespace Vote.Monitor.Api.Feature.PollingStation.Notes.List;

public record Response
{
    public required List<NoteModel> Notes { get; init; }
}
