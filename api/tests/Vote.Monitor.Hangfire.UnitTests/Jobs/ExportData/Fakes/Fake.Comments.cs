using Vote.Monitor.Hangfire.Jobs.Export.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData.Fakes;

public sealed partial class Fake
{
    public const string FormattedComments =
        "Jane Doe (2026-10-17T10:00:00): Some comment\n\nJohn Doe (2026-10-17T11:00:00): Another comment";

    public static CommentModel[] Comments() =>
    [
        new() { AuthorName = "Jane Doe", Text = "Some comment", Timestamp = new DateTimeOffset(2026, 10, 17, 12, 0, 0, TimeSpan.FromHours(2)) },
        new() { AuthorName = "John Doe", Text = "Another comment", Timestamp = new DateTimeOffset(2026, 10, 17, 11, 0, 0, TimeSpan.Zero) }
    ];
}
