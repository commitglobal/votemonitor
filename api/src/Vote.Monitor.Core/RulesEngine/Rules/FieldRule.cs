using System.Text.Json;

namespace Vote.Monitor.Core.RulesEngine.Rules;

/// <summary>A rule about a field: its name is in the header list (docs/advanced-filters-backend.md,
/// section 3), not a column name. The value keys follow the op — <c>value</c> for one value,
/// <c>values</c> for a list, <c>from</c> and <c>to</c> for a range; isEmpty/isNotEmpty have none.</summary>
public abstract record FieldRule : FilterRule
{
    [JsonPropertyName("field")]
    public required string Field { get; init; }

    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Value { get; init; }

    [JsonPropertyName("values")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<JsonElement>? Values { get; init; }

    [JsonPropertyName("from")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? From { get; init; }

    [JsonPropertyName("to")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? To { get; init; }
}