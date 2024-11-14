using Ardalis.SmartEnum;

namespace Vote.Monitor.Core.Models;

[JsonConverter(typeof(SmartEnumValueConverter<DataSource, string>))]
public sealed class DataSource : SmartEnum<DataSource, string>
{
    public static readonly DataSource Ngo = new(nameof(Ngo), nameof(Ngo));
    public static readonly DataSource Coalition = new(nameof(Coalition), nameof(Coalition));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="DataSource" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out DataSource result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private DataSource(string name, string value) : base(name, value)
    {
    }
}
