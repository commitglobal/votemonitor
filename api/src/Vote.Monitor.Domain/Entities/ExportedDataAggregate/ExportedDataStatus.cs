using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.ExportedDataAggregate;
public sealed class ExportedDataStatus : SmartEnum<ExportedDataStatus, string>
{
    public static readonly ExportedDataStatus Started = new(nameof(Started), nameof(Started));
    public static readonly ExportedDataStatus Failed = new(nameof(Failed), nameof(Failed));
    public static readonly ExportedDataStatus Completed = new(nameof(Completed), nameof(Completed));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="ExportedDataStatus" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out ExportedDataStatus result)
    {
        return TryFromValue(value, out result);
    }

    private ExportedDataStatus(string name, string value) : base(name, value)
    {
    }
}
