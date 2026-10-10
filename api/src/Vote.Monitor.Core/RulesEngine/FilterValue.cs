using System.Globalization;
using System.Text.Json;
using Ardalis.SmartEnum;

namespace Vote.Monitor.Core.RulesEngine;

public static class FilterValue
{
    public static object? String(JsonElement value) =>
        value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => null,
            _ => throw new ArgumentException("Expected a scalar string value.")
        };

    public static object? Boolean(JsonElement value) =>
        value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String when bool.TryParse(value.GetString(), out var b) => b,
            _ => throw new ArgumentException("Expected a boolean value.")
        };

    public static object? Guid(JsonElement value)
    {
        var text = value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : value.GetRawText();

        return System.Guid.TryParse(text, out var id)
            ? id
            : throw new ArgumentException("Expected a valid UUID.");
    }

    public static object? Integer(JsonElement value)
    {
        var text = value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : value.GetRawText();

        return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
            ? n
            : throw new ArgumentException("Expected an integer.");
    }

    public static object? Decimal(JsonElement value)
    {
        var text = value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : value.GetRawText();

        return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var n)
            ? n
            : throw new ArgumentException("Expected a number.");
    }

    public static object? UtcDateTimeOffset(JsonElement value)
    {
        if (value.ValueKind != JsonValueKind.String ||
            !DateTimeOffset.TryParse(
                value.GetString(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var result) ||
            result.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Expected an ISO 8601 UTC timestamp.");
        }

        return result;
    }

    public static object? Date(JsonElement value)
    {
        var text = value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : value.GetRawText();

        return DateOnly.TryParseExact(
            text,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date)
            ? date
            : throw new ArgumentException("Expected a date in yyyy-MM-dd format.");
    }

    public static Func<JsonElement, object?> Enum<TEnum>()
        where TEnum : SmartEnum<TEnum, string>
    {
        return value =>
        {
            if (value.ValueKind != JsonValueKind.String)
            {
                throw new ArgumentException(
                    $"Expected a valid {typeof(TEnum).Name} value.");
            }

            try
            {
                var result = SmartEnum<TEnum, string>.FromName(
                    value.GetString()!,
                    ignoreCase: false);

                return result.Value;
            }
            catch (SmartEnumNotFoundException)
            {
                throw new ArgumentException(
                    $"Expected a valid {typeof(TEnum).Name} value.");
            }
        };
    }
}
