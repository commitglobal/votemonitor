namespace Vote.Monitor.Core.RulesEngine.Rules;

/// <summary>and / or / not: their children as rules (a "not" holds a single group rule).</summary>
public abstract record LogicalFilter : FilterRule
{
    [JsonPropertyName("values")]
    [JsonConverter(typeof(FilterRuleListConverter))]
    public List<FilterRule>? Values { get; init; }
}