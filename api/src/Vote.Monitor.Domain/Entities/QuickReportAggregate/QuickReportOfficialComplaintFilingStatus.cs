using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.QuickReportAggregate;

public sealed class
    QuickReportOfficialComplaintFilingStatus : SmartEnum<QuickReportOfficialComplaintFilingStatus, string>
{
    public static readonly QuickReportOfficialComplaintFilingStatus Yes = new(nameof(Yes), nameof(Yes));

    public static readonly QuickReportOfficialComplaintFilingStatus NoButPlanningToFile =
        new(nameof(NoButPlanningToFile), nameof(NoButPlanningToFile));

    public static readonly QuickReportOfficialComplaintFilingStatus NoAndNotPlanningToFile =
        new(nameof(NoAndNotPlanningToFile), nameof(NoAndNotPlanningToFile));

    public static readonly QuickReportOfficialComplaintFilingStatus DoesNotApplyOrOther =
        new(nameof(DoesNotApplyOrOther), nameof(DoesNotApplyOrOther));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="QuickReportOfficialComplaintFilingStatus" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out QuickReportOfficialComplaintFilingStatus result)
    {
        return TryFromValue(value, out result);
    }

    private QuickReportOfficialComplaintFilingStatus(string name, string value) : base(name, value)
    {
    }
}