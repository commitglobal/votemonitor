﻿namespace Vote.Monitor.Domain.Entities.CitizenGuideAggregate;

[JsonConverter(typeof(SmartEnumValueConverter<CitizenGuideType, string>))]
public sealed class CitizenGuideType : SmartEnum<CitizenGuideType, string>
{
    public static readonly CitizenGuideType Website = new(nameof(Website), nameof(Website));
    public static readonly CitizenGuideType Document = new(nameof(Document), nameof(Document));
    public static readonly CitizenGuideType Text = new(nameof(Text), nameof(Text));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="CitizenGuideType" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out CitizenGuideType result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private CitizenGuideType(string name, string value) : base(name, value)
    {
    }
}
