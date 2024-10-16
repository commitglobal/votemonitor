namespace Vote.Monitor.Domain.Entities.CitizenReportAggregate;

[JsonConverter(typeof(SmartEnumValueConverter<CitizenReportFollowUpStatus, string>))]
public sealed class CitizenReportFollowUpStatus : SmartEnum<CitizenReportFollowUpStatus, string>
{
    public static readonly CitizenReportFollowUpStatus NotApplicable = new(nameof(NotApplicable), nameof(NotApplicable));
    public static readonly CitizenReportFollowUpStatus NeedsFollowUp = new(nameof(NeedsFollowUp), nameof(NeedsFollowUp));
    public static readonly CitizenReportFollowUpStatus Resolved = new(nameof(Resolved), nameof(Resolved));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="CitizenReportFollowUpStatus" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out CitizenReportFollowUpStatus result)
    {
        return TryFromValue(value, out result);
    }

    private CitizenReportFollowUpStatus(string name, string value) : base(name, value)
    {
    }
}
