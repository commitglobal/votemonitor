using System.Text.Json;

namespace Vote.Monitor.Core.RulesEngine.Rules;

/// <summary>The items of a logical rule: every rule (a group or a single rule) has an <c>op</c>,
/// so a list has nothing else to disambiguate.</summary>
public sealed class FilterRuleListConverter : JsonConverter<List<FilterRule>>
{
    public override List<FilterRule> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("A logical rule's values must be an array.");

        var items = new List<FilterRule>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("A rule must be an object.");
            using var document = JsonDocument.ParseValue(ref reader);
            items.Add(document.RootElement.Deserialize<FilterRule>(options)!);
        }
        return items;
    }

    public override void Write(Utf8JsonWriter writer, List<FilterRule> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var item in value)
            JsonSerializer.Serialize(writer, item, item.GetType(), options);
        writer.WriteEndArray();
    }
}
