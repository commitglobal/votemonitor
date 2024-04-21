namespace Feature.Notes.List;

public record Response
{
    public required List<NoteModel> Notes { get; init; }
}
