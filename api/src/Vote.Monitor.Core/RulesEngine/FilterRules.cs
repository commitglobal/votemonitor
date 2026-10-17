using System.Text.Json;
using Vote.Monitor.Core.RulesEngine.Rules;

namespace Vote.Monitor.Core.RulesEngine;

/// <summary>
/// Builds filter rules in code, e.g. to combine legacy query parameters with a request's filter.
/// </summary>
public static class FilterRules
{
    /// <summary>
    /// <c>field = value</c>, or null when there is no value (so an unset optional parameter adds no rule).
    /// Blank strings count as no value.
    /// </summary>
    public static FilterRule? Eq(string field, object? value)
    {
        if (value is null || value is string text && string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return new EqFilter { Field = field, Value = JsonSerializer.SerializeToElement(value) };
    }

    /// <summary>
    /// All the given rules, ignoring nulls; null when there are none.
    /// </summary>
    public static FilterRule? And(params FilterRule?[] rules)
    {
        var values = rules.OfType<FilterRule>().ToList();
        return values.Count switch
        {
            0 => null,
            1 => values[0],
            _ => new AndFilter { Values = values }
        };
    }
}
