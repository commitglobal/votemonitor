using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.QuickReportAggregate;

public sealed class QuickReportIssueType : SmartEnum<QuickReportIssueType, string>
{
    public static readonly QuickReportIssueType A = new(nameof(A), nameof(A));
    public static readonly QuickReportIssueType B = new(nameof(B), nameof(B));
    public static readonly QuickReportIssueType C = new(nameof(C), nameof(C));
    public static readonly QuickReportIssueType D = new(nameof(D), nameof(D));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="QuickReportIssueType" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out QuickReportIssueType result)
    {
        return TryFromValue(value, out result);
    }

    private QuickReportIssueType(string name, string value) : base(name, value)
    {
    }
}