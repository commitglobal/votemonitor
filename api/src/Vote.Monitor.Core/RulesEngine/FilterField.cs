using System.Text.Json;

namespace Vote.Monitor.Core.RulesEngine;

/// <param name="IgnoreCase">'contains' and 'notContains' ignore case (ILIKE), e.g. for names and emails.</param>
public sealed record FilterField(
    string Sql,
    string SqlType,
    IReadOnlySet<string> Operators,
    Func<JsonElement, object?> ConvertValue,
    string? EmptySql = null,
    bool IsArray = false,
    bool IgnoreCase = false);