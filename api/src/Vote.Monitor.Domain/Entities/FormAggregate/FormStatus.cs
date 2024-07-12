using System.Text.Json.Serialization;
using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormAggregate;

public sealed class FormStatus : SmartEnum<FormStatus, string>
{
    public static readonly FormStatus Drafted = new(nameof(Drafted), nameof(Drafted));
    public static readonly FormStatus Published = new(nameof(Published), nameof(Published));
    public static readonly FormStatus Obsolete = new(nameof(Obsolete), nameof(Obsolete));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="FormStatus" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out FormStatus result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private FormStatus(string name, string value) : base(name, value)
    {
    }
}
