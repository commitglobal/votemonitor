namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>What a report activity entry records.</summary>
[JsonConverter(typeof(SmartEnumValueConverter<ReviewActivityKind, string>))]
public sealed class ReviewActivityKind : SmartEnum<ReviewActivityKind, string>
{
    public static readonly ReviewActivityKind StageChanged = new(nameof(StageChanged), nameof(StageChanged));
    public static readonly ReviewActivityKind ReviewerAdded = new(nameof(ReviewerAdded), nameof(ReviewerAdded));
    public static readonly ReviewActivityKind ReviewerRemoved = new(nameof(ReviewerRemoved), nameof(ReviewerRemoved));
    public static readonly ReviewActivityKind FollowUpStatusChanged = new(nameof(FollowUpStatusChanged), nameof(FollowUpStatusChanged));
    public static readonly ReviewActivityKind SeverityChanged = new(nameof(SeverityChanged), nameof(SeverityChanged));

    /// <summary>The observer or citizen who submitted the report changed it, so it went back to the default stage.</summary>
    public static readonly ReviewActivityKind ReturnedToDefaultStage = new(nameof(ReturnedToDefaultStage), nameof(ReturnedToDefaultStage));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    public static bool TryParse(string value, out ReviewActivityKind result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private ReviewActivityKind(string name, string value) : base(name, value)
    {
    }
}
