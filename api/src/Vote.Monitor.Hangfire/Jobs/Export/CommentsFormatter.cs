using Vote.Monitor.Hangfire.Jobs.Export.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export;

public static class CommentsFormatter
{
    public const string ColumnHeader = "Comments";
    private const string Separator = "\n\n";

    /// <summary>
    /// Formats comments as "author (timestamp): text", separated by an empty line.
    /// </summary>
    public static string Format(CommentModel[]? comments)
    {
        return string.Join(Separator,
            (comments ?? []).Select(c => $"{c.AuthorName} ({c.Timestamp.UtcDateTime:s}): {c.Text}"));
    }
}
