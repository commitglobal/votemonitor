using System.Text.Json.Serialization;
using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormAggregate;

public sealed class FormType : SmartEnum<FormType, string>
{
    public static readonly FormType Opening = new(nameof(Opening), nameof(Opening));
    public static readonly FormType Voting = new(nameof(Voting), nameof(Voting));
    public static readonly FormType ClosingAndCounting = new(nameof(ClosingAndCounting), nameof(ClosingAndCounting));
    public static readonly FormType PSI = new(nameof(PSI), nameof(PSI));
    public static readonly FormType Other = new(nameof(Other), nameof(Other));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="FormType" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out FormType result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private FormType(string name, string value) : base(name, value)
    {
    }
}
