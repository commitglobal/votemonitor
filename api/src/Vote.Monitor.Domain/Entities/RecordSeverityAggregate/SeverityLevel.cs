namespace Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

/// <summary>How severe a monitoring NGO considers a report, ascending from <see cref="None"/> to <see cref="Urgent"/>.</summary>
[JsonConverter(typeof(SmartEnumValueConverter<SeverityLevel, string>))]
public sealed class SeverityLevel : SmartEnum<SeverityLevel, string>
{
    public static readonly SeverityLevel None = new(nameof(None), nameof(None), 0);
    public static readonly SeverityLevel Low = new(nameof(Low), nameof(Low), 1);
    public static readonly SeverityLevel Medium = new(nameof(Medium), nameof(Medium), 2);
    public static readonly SeverityLevel High = new(nameof(High), nameof(High), 3);
    public static readonly SeverityLevel Urgent = new(nameof(Urgent), nameof(Urgent), 4);

    /// <summary>The position in the scale, 0 (None) to 4 (Urgent).</summary>
    public int Level { get; }

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    public static bool TryParse(string value, out SeverityLevel result)
    {
        return TryFromValue(value, out result);
    }

    private SeverityLevel(string name, string value, int level) : base(name, value)
    {
        Level = level;
    }
}
