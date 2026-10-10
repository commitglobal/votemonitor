using System.Text.Json;
using Vote.Monitor.Core.RulesEngine.Rules;

namespace Vote.Monitor.Core.RulesEngine;

public static class FilterRuleJson
{
    public static FilterRule? Deserialize(JsonDocument? document)
    {
        if (document is null)
        {
            return null;
        }

        return document.RootElement.Deserialize<FilterRule>();
    }

    public static JsonDocument? Serialize(FilterRule? rule)
    {
        return rule is null ? null : JsonSerializer.SerializeToDocument(rule);
    }
}
