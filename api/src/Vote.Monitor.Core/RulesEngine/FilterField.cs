using System.Text.Json;

namespace Vote.Monitor.Core.RulesEngine;

public sealed record FilterField(
    string Sql,
    string SqlType,
    IReadOnlySet<string> Operators,
    Func<JsonElement, object?> ConvertValue,
    string? EmptySql = null);