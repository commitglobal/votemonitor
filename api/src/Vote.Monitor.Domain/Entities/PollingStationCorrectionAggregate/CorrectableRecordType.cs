namespace Vote.Monitor.Domain.Entities.PollingStationCorrectionAggregate;

[JsonConverter(typeof(SmartEnumValueConverter<CorrectableRecordType, string>))]
public sealed class CorrectableRecordType : SmartEnum<CorrectableRecordType, string>
{
    public static readonly CorrectableRecordType FormSubmission = new(nameof(FormSubmission), nameof(FormSubmission));
    public static readonly CorrectableRecordType QuickReport = new(nameof(QuickReport), nameof(QuickReport));
    public static readonly CorrectableRecordType IncidentReport = new(nameof(IncidentReport), nameof(IncidentReport));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    public static bool TryParse(string value, out CorrectableRecordType result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private CorrectableRecordType(string name, string value) : base(name, value)
    {
    }
}
