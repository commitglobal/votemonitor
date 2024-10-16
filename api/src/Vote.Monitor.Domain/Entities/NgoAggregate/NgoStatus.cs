namespace Vote.Monitor.Domain.Entities.NgoAggregate;

[JsonConverter(typeof(SmartEnumValueConverter<NgoStatus, string>))]
public sealed class NgoStatus : SmartEnum<NgoStatus, string>
{
    public static readonly NgoStatus Activated = new(nameof(Activated), nameof(Activated));
    public static readonly NgoStatus Deactivated = new(nameof(Deactivated), nameof(Deactivated));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="NgoStatus" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out NgoStatus result)
    {
        return TryFromValue(value, out result);
    }

    private NgoStatus(string name, string value) : base(name, value)
    {
    }
}
