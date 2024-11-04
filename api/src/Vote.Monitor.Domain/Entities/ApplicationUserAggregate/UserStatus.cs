namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

[JsonConverter(typeof(SmartEnumValueConverter<UserStatus, string>))]
public sealed class UserStatus : SmartEnum<UserStatus, string>
{
    public static readonly UserStatus Active = new(nameof(Active), nameof(Active));
    public static readonly UserStatus Deactivated = new(nameof(Deactivated), nameof(Deactivated));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="UserStatus" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out UserStatus result)
    {
        return TryFromValue(value, out result);
    }
    
    [JsonConstructor]
    private UserStatus(string name, string value) : base(name, value)
    {
    }
}
