namespace Vote.Monitor.Hangfire.Jobs.Export.ReadModels;

public class CommentModel
{
    public string AuthorName { get; init; } = default!;
    public string Text { get; init; } = default!;
    public DateTimeOffset Timestamp { get; init; }
}
