namespace Vote.Monitor.Domain.Entities.IncidentReportAggregate;

[JsonConverter(typeof(SmartEnumValueConverter<IncidentReportLocationType, string>))]
public sealed class IncidentReportLocationType : SmartEnum<IncidentReportLocationType, string>
{
    public static readonly IncidentReportLocationType PollingStation = new(nameof(PollingStation), nameof(PollingStation));

    public static readonly IncidentReportLocationType OtherLocation = new(nameof(OtherLocation), nameof(OtherLocation));


    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="IncidentReportLocationType" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out IncidentReportLocationType result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private IncidentReportLocationType(string name, string value) : base(name, value)
    {
    }
}
