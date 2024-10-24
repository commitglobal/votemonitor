﻿namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

[JsonConverter(typeof(SmartEnumValueConverter<FormTemplateType, string>))]
public sealed class FormTemplateType : SmartEnum<FormTemplateType, string>
{
    public static readonly FormTemplateType Opening = new(nameof(Opening), nameof(Opening));
    public static readonly FormTemplateType Voting = new(nameof(Voting), nameof(Voting));
    public static readonly FormTemplateType ClosingAndCounting = new(nameof(ClosingAndCounting), nameof(ClosingAndCounting));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="FormTemplateType" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out FormTemplateType result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private FormTemplateType(string name, string value) : base(name, value)
    {
    }
}
