namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>The kinds of report that carry a workflow stage and reviewers.</summary>
[JsonConverter(typeof(SmartEnumValueConverter<ReportRecordType, string>))]
public sealed class ReportRecordType : SmartEnum<ReportRecordType, string>
{
    public static readonly ReportRecordType FormSubmission = new(nameof(FormSubmission), nameof(FormSubmission));
    public static readonly ReportRecordType QuickReport = new(nameof(QuickReport), nameof(QuickReport));
    public static readonly ReportRecordType IncidentReport = new(nameof(IncidentReport), nameof(IncidentReport));
    public static readonly ReportRecordType CitizenReport = new(nameof(CitizenReport), nameof(CitizenReport));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    public static bool TryParse(string value, out ReportRecordType result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private ReportRecordType(string name, string value) : base(name, value)
    {
    }
}
