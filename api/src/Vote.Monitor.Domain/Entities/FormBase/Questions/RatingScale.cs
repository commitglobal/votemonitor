namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

[JsonConverter(typeof(SmartEnumValueConverter<RatingScale, string>))]
public class RatingScale : SmartEnum<RatingScale, string>
{
    public int UpperBound { get; }
    
    public static readonly RatingScale OneTo3 = new(nameof(OneTo3), nameof(OneTo3), 3);
    public static readonly RatingScale OneTo4 = new(nameof(OneTo4), nameof(OneTo4), 4);
    public static readonly RatingScale OneTo5 = new(nameof(OneTo5), nameof(OneTo5), 5);
    public static readonly RatingScale OneTo6 = new(nameof(OneTo6), nameof(OneTo6), 6);
    public static readonly RatingScale OneTo7 = new(nameof(OneTo7), nameof(OneTo7), 7);
    public static readonly RatingScale OneTo8 = new(nameof(OneTo8), nameof(OneTo8), 8);
    public static readonly RatingScale OneTo9 = new(nameof(OneTo9), nameof(OneTo9), 9);
    public static readonly RatingScale OneTo10 = new(nameof(OneTo10), nameof(OneTo10), 10);

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="RatingScale" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out RatingScale result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private RatingScale(string name, string value, int upperBound) : base(name, value)
    {
        UpperBound = upperBound;
    }

    public int[] ToRange()
    {
        return Enumerable.Range(1, UpperBound).ToArray();
    }
}
