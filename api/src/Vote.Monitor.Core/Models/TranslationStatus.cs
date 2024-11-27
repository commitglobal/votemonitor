using Ardalis.SmartEnum;

namespace Vote.Monitor.Core.Models;

[JsonConverter(typeof(SmartEnumValueConverter<TranslationStatus, string>))]
public sealed class TranslationStatus : SmartEnum<TranslationStatus, string>
{
    public static readonly TranslationStatus Translated = new(nameof(Translated), nameof(Translated));
    public static readonly TranslationStatus MissingTranslations = new(nameof(MissingTranslations), nameof(MissingTranslations));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="TranslationStatus" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out TranslationStatus result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private TranslationStatus(string name, string value) : base(name, value)
    {
    }
}
