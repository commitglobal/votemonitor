namespace Vote.Monitor.Core.Models;

/// <summary>
/// The body of a 409 for a save made from an older version of a record (concurrent edits contract): nothing was saved,
/// <see cref="Current"/> is the record exactly as its GET returns it now.
/// </summary>
public class VersionConflict<T>
{
    public const string ConflictTitle = "This record was changed by someone else";

    public string Title { get; init; } = ConflictTitle;
    public int Status { get; init; } = 409;
    public required T Current { get; init; }

    public static VersionConflict<T> Of(T current) => new() { Current = current };
}
