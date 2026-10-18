namespace Vote.Monitor.Domain.Entities.IncidentReportAggregate;

[JsonConverter(typeof(IncidentReportLocationTypeConverter))]
public sealed class IncidentReportLocationType : SmartEnum<IncidentReportLocationType, string>
{
    public static readonly IncidentReportLocationType PollingStation = new(nameof(PollingStation), nameof(PollingStation));

    public static readonly IncidentReportLocationType OtherPollingStation =
        new(nameof(OtherPollingStation), nameof(OtherPollingStation));

    public static readonly IncidentReportLocationType NotRelatedToAPollingStation =
        new(nameof(NotRelatedToAPollingStation), nameof(NotRelatedToAPollingStation));

    /// <summary>The value <see cref="NotRelatedToAPollingStation"/> had before it was renamed; still accepted from clients.</summary>
    public const string LegacyOtherLocation = "OtherLocation";

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
        if (value == LegacyOtherLocation)
        {
            result = NotRelatedToAPollingStation;
            return true;
        }

        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private IncidentReportLocationType(string name, string value) : base(name, value)
    {
    }
}

/// <summary>Reads and writes <see cref="IncidentReportLocationType"/> by value, accepting the legacy <c>OtherLocation</c>.</summary>
public sealed class IncidentReportLocationTypeConverter : JsonConverter<IncidentReportLocationType>
{
    public override IncidentReportLocationType? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing a {nameof(IncidentReportLocationType)}.");
        }

        var value = reader.GetString()!;

        return IncidentReportLocationType.TryParse(value, out var result)
            ? result
            : throw new JsonException($"Error converting value '{value}' to a {nameof(IncidentReportLocationType)}.");
    }

    public override void Write(Utf8JsonWriter writer, IncidentReportLocationType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
