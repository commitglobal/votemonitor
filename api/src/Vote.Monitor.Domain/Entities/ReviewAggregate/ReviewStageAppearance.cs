namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>The colours and icons a review stage can pick from; the web app draws each key.</summary>
public static class ReviewStageAppearance
{
    public static readonly IReadOnlySet<string> Colors = new HashSet<string>
    {
        "gray", "brown", "orange", "yellow", "lime", "green", "teal", "cyan", "blue", "indigo", "purple", "pink", "red"
    };

    public static readonly IReadOnlySet<string> Icons = new HashSet<string>
    {
        "circle", "circle-dashed", "circle-dot", "clock", "hourglass", "eye", "search", "phone", "message",
        "user-check", "users", "flag", "alert", "shield", "check", "check-check", "x", "archive", "star", "bookmark",
        "send", "pause"
    };
}
