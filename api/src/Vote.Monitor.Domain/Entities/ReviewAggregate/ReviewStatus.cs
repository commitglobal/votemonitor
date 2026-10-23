namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>The fixed status every report stage belongs to.</summary>
[JsonConverter(typeof(SmartEnumValueConverter<ReviewStatus, string>))]
public sealed class ReviewStatus : SmartEnum<ReviewStatus, string>
{
    public static readonly ReviewStatus NotReviewed = new(nameof(NotReviewed), nameof(NotReviewed));
    public static readonly ReviewStatus InReview = new(nameof(InReview), nameof(InReview));
    public static readonly ReviewStatus Reviewed = new(nameof(Reviewed), nameof(Reviewed));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    public static bool TryParse(string value, out ReviewStatus result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private ReviewStatus(string name, string value) : base(name, value)
    {
    }
}
