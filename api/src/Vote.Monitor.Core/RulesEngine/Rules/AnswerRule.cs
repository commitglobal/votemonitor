using System.Text.Json;

namespace Vote.Monitor.Core.RulesEngine.Rules;

/// <summary>A rule about the answer to one question; it never has a field, and its value keys
/// follow the question type as on the field rules above.</summary>
public abstract record AnswerRule : FilterRule
{
    [JsonPropertyName("form")]
    public required Guid Form { get; init; }

    [JsonPropertyName("question")]
    public required Guid Question { get; init; }

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
