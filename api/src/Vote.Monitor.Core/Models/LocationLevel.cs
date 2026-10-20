using Ardalis.SmartEnum;

namespace Vote.Monitor.Core.Models;

/// <summary>
/// A level of the Level1..Level5 location path of polling stations and citizen report locations.
/// </summary>
[JsonConverter(typeof(SmartEnumValueConverter<LocationLevel, string>))]
[SmartEnumStringComparer(StringComparison.InvariantCultureIgnoreCase)]
public sealed class LocationLevel : SmartEnum<LocationLevel, string>
{
    public static readonly LocationLevel Level1 = new(nameof(Level1), nameof(Level1), 1);
    public static readonly LocationLevel Level2 = new(nameof(Level2), nameof(Level2), 2);
    public static readonly LocationLevel Level3 = new(nameof(Level3), nameof(Level3), 3);
    public static readonly LocationLevel Level4 = new(nameof(Level4), nameof(Level4), 4);
    public static readonly LocationLevel Level5 = new(nameof(Level5), nameof(Level5), 5);

    /// <summary>
    /// 1..5: how deep the level is in the location path.
    /// </summary>
    public int Depth { get; }

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="LocationLevel" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out LocationLevel result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private LocationLevel(string name, string value, int depth) : base(name, value)
    {
        Depth = depth;
    }
}
