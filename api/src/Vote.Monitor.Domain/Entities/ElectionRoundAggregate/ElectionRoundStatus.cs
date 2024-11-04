namespace Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

[JsonConverter(typeof(SmartEnumValueConverter<ElectionRoundStatus, string>))]
public sealed class ElectionRoundStatus : SmartEnum<ElectionRoundStatus, string>
{
    public static readonly ElectionRoundStatus NotStarted = new(nameof(NotStarted), nameof(NotStarted));
    public static readonly ElectionRoundStatus Started = new(nameof(Started), nameof(Started));
    public static readonly ElectionRoundStatus Archived = new(nameof(Archived), nameof(Archived));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="ElectionRoundStatus" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out ElectionRoundStatus result)
    {
        return TryFromValue(value, out result);
    }
    
    [JsonConstructor]
    private ElectionRoundStatus(string name, string value) : base(name, value)
    {
    }
}
