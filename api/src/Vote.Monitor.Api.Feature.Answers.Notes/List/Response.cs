namespace Vote.Monitor.Api.Feature.Answers.Notes.List;

public record Response
{
    public required List<NoteModel> Notes { get; init; }
}
