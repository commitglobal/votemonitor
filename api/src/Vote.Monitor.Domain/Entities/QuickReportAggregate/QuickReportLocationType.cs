namespace Vote.Monitor.Domain.Entities.QuickReportAggregate;

[JsonConverter(typeof(SmartEnumValueConverter<QuickReportLocationType, string>))]
public sealed class QuickReportLocationType : SmartEnum<QuickReportLocationType, string>
{
    public static readonly QuickReportLocationType VisitedPollingStation =
        new(nameof(VisitedPollingStation), nameof(VisitedPollingStation));

    public static readonly QuickReportLocationType OtherPollingStation =
        new(nameof(OtherPollingStation), nameof(OtherPollingStation));

    public static readonly QuickReportLocationType NotRelatedToAPollingStation =
        new(nameof(NotRelatedToAPollingStation), nameof(NotRelatedToAPollingStation));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="QuickReportLocationType" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out QuickReportLocationType result)
    {
        return TryFromValue(value, out result);
    }

    private QuickReportLocationType(string name, string value) : base(name, value)
    {
    }
}