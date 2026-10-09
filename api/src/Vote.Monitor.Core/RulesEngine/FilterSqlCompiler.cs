using System.Text.Json;
using Dapper;
using Vote.Monitor.Core.RulesEngine.Rules;

namespace Vote.Monitor.Core.RulesEngine;

public sealed class FilterSqlCompiler
{
    private readonly IReadOnlyDictionary<string, FilterField> _fields;
    private readonly Func<AnswerRule, FilterSqlCompiler, string>? _compileAnswer;
    private DynamicParameters _parameters = new();
    private int _parameterIndex;

    public FilterSqlCompiler(
        IReadOnlyDictionary<string, FilterField> fields,
        Func<AnswerRule, FilterSqlCompiler, string>? compileAnswer = null)
    {
        _fields = fields;
        _compileAnswer = compileAnswer;
    }

    public CompiledFilter Build(FilterRule? rule)
    {
        _parameters = new DynamicParameters();

        _parameterIndex = 0;

        return new CompiledFilter(
            rule is null ? "1 = 1" : Compile(rule),
            _parameters);
    }

    private string Compile(FilterRule rule) =>
        rule switch
        {
            AndFilter and => CompileGroup(and.Values, "AND"),
            OrFilter or => CompileGroup(or.Values, "OR"),
            NotFilter not => CompileNot(not),
            FieldRule field => CompileField(field),
            AnswerRule answer => CompileAnswerRule(answer),
            _ => throw new ArgumentException(
                $"Unsupported filter rule: {rule.GetType().Name}.")
        };

    private string CompileGroup(List<FilterRule>? rules, string joiner)
    {
        if (rules is null)
            throw new ArgumentException("Logical rule must contain a values array.");

        if (rules.Count == 0)
            return joiner == "AND" ? "TRUE" : "FALSE";

        return "(" + string.Join(
            $" {joiner} ",
            rules.Select(Compile)) + ")";
    }

    private string CompileNot(NotFilter rule)
    {
        if (rule.Values is not { Count: 1 })
            throw new ArgumentException("'not' must contain exactly one rule.");

        return $"(NOT {Compile(rule.Values[0])})";
    }

    private string CompileField(FieldRule rule)
    {
        if (!_fields.TryGetValue(rule.Field, out var field))
            throw new ArgumentException($"Filtering by '{rule.Field}' is not allowed.");

        var op = rule.Discriminator;

        if (!field.Operators.Contains(op))
            throw new ArgumentException(
                $"Operator '{op}' is not supported for field '{rule.Field}'.");

        var column = field.Sql;

        return op switch
        {
            FilterOps.Eq => $"{column} = {AddRequired(rule.Value, field)}",
            FilterOps.Ne => $"{column} <> {AddRequired(rule.Value, field)}",
            FilterOps.Lt => $"{column} < {AddRequired(rule.Value, field)}",
            FilterOps.Lte => $"{column} <= {AddRequired(rule.Value, field)}",
            FilterOps.Gt => $"{column} > {AddRequired(rule.Value, field)}",
            FilterOps.Gte => $"{column} >= {AddRequired(rule.Value, field)}",

            FilterOps.Between => CompileBetween(
                column, field, rule.From, rule.To),

            FilterOps.In => CompileIn(
                column, field, rule.Values, negate: false),

            FilterOps.NotIn => CompileIn(
                column, field, rule.Values, negate: true),

            FilterOps.Contains => CompileContains(
                column, field, rule.Value, negate: false),

            FilterOps.NotContains => CompileContains(
                column, field, rule.Value, negate: true),

            FilterOps.IsEmpty => CompileEmpty(field, isEmpty: true),
            FilterOps.IsNotEmpty => CompileEmpty(field, isEmpty: false),

            _ => throw new ArgumentException($"Unsupported operator '{op}'.")
        };
    }

    private string CompileAnswerRule(AnswerRule rule)
    {
        string? value1 = "NULL::jsonb";
        string? value2 = "NULL::jsonb";

        switch (rule.Discriminator)
        {
            case "answer-isEmpty":
            case "answer-isNotEmpty":
                break;

            case "answer-between":
                value1 = AddParameter(rule.From);
                value2 = AddParameter(rule.To);
                break;

            case "answer-in":
            case "answer-notIn":
                value1 = rule.Values is not null
                    ? AddParameter(rule.Values)
                    : AddParameter([]);
                break;

            default:
                // Supports scalar values and array-valued equality for multi-select.
                value1 = rule.Values is not null
                    ? AddParameter(rule.Values)
                    : AddParameter(rule.Value);
                break;
        }

        string[] parts =
        [
            $"""
             "FormId" = {AddParameter(rule.Form)}
             """,
            $"""
             "AnswerMatches"(
                 "Answers",
                 {AddParameter(rule.Question)},
                 {AddParameter(rule.Discriminator)},
                 {value1},
                 {value2}
             )
             """
        ];

        return "(" + string.Join(" AND ", parts) + ")";
    }
 

    private string CompileBetween(
        string column,
        FilterField field,
        JsonElement? from,
        JsonElement? to)
    {
        var parts = new List<string>();

        if (from is { } lower && lower.ValueKind != JsonValueKind.Null)
            parts.Add($"{column} >= {AddParameter(lower, field)}");

        if (to is { } upper && upper.ValueKind != JsonValueKind.Null)
            parts.Add($"{column} <= {AddParameter(upper, field)}");

        if (parts.Count == 0)
            throw new ArgumentException(
                "'between' requires at least one of 'from' or 'to'.");

        return "(" + string.Join(" AND ", parts) + ")";
    }

    private string CompileIn(
        string column,
        FilterField field,
        List<JsonElement>? values,
        bool negate)
    {
        if (values is null)
            throw new ArgumentException("'in' and 'notIn' require a values array.");

        // Avoid invalid SQL "IN ()" and give empty lists predictable semantics.
        if (values.Count == 0)
            return negate ? "TRUE" : "FALSE";

        var parameters = values
            .Select(value => AddParameter(value, field))
            .ToArray();

        return $"{column} {(negate ? "NOT IN" : "IN")} ({string.Join(", ", parameters)})";
    }

    private string CompileContains(
        string column,
        FilterField field,
        JsonElement? value,
        bool negate)
    {
        if (field.SqlType != "text")
            throw new ArgumentException("'contains' requires a text field.");

        var raw = RequireValue(value);

        if (raw.ValueKind != JsonValueKind.String)
            throw new ArgumentException("'contains' requires a string value.");

        // Escape LIKE metacharacters so the input is treated as literal text.
        var escaped = raw.GetString()!
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");

        var name = $"filter_{_parameterIndex++}";
        _parameters.Add(name, $"%{escaped}%");

        return $"{column} {(negate ? "NOT LIKE" : "LIKE")} @{name} ESCAPE '\\'";
    }

    private static string CompileEmpty(FilterField field, bool isEmpty)
    {
        if (field.EmptySql is not null)
            return isEmpty
                ? $"({field.EmptySql})"
                : $"(NOT ({field.EmptySql}))";

        return isEmpty
            ? $"({field.Sql} IS NULL)"
            : $"({field.Sql} IS NOT NULL)";
    }

    private string AddRequired(JsonElement? value, FilterField field) =>
        AddParameter(RequireValue(value), field);

    private string AddParameter(JsonElement value, FilterField field)
    {
        if (value.ValueKind == JsonValueKind.Null)
            throw new ArgumentException("Null is not a valid comparison value.");

        var converted = field.ConvertValue(value);
        var name = $"filter_{_parameterIndex++}";

        _parameters.Add(name, converted);

        // SqlType comes only from server-side field configuration.
        return $"@{name}::{field.SqlType}";
    }
    
    private string AddParameter(Guid value)
    {
        var name = $"filter_{_parameterIndex++}";
        _parameters.Add(name, value);
        return $"@{name}::uuid";
    }
    
    private string AddParameter(string value)
    {
        var name = $"filter_{_parameterIndex++}";
        _parameters.Add(name, value);
        return $"@{name}::text";
    }
    
    private string AddParameter(JsonElement? value)
    {
        var name = $"filter_{_parameterIndex++}";
        if (value == null || value.Value.ValueKind == JsonValueKind.Null)
        {
            _parameters.Add(name, null);
        }
        else
        {
            _parameters.Add(name, JsonSerializer.Serialize(value));
        }
        
        return $"@{name}::jsonb";
    }
    
    private string AddParameter(List<JsonElement>? values)
    {
        var name = $"filter_{_parameterIndex++}";
        if (values == null)
        {
            _parameters.Add(name, null);
        }
        else
        {
            _parameters.Add(name, JsonSerializer.Serialize(values));
        }
        
        return $"@{name}::jsonb";
    }

    private static JsonElement RequireValue(JsonElement? value)
    {
        if (value is null || value.Value.ValueKind == JsonValueKind.Null)
            throw new ArgumentException("This operator requires a value.");

        return value.Value;
    }

    // Used by a schema-specific answer compiler to add safely parameterized values.
    public string Parameter(JsonElement value, string postgresType,
        Func<JsonElement, object?> convert)
    {
        var name = $"filter_{_parameterIndex++}";
        _parameters.Add(name, convert(value));
        return $"@{name}::{postgresType}";
    }

    public string Parameter(Guid value)
    {
        var name = $"filter_{_parameterIndex++}";
        _parameters.Add(name, value);
        return $"@{name}::uuid";
    }
}
