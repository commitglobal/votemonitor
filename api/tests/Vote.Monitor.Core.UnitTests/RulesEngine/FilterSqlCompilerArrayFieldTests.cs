using System.Text.Json;
using System.Text.Json.Nodes;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;

namespace Vote.Monitor.Core.UnitTests.RulesEngine;

public class FilterSqlCompilerArrayFieldTests
{
    private static readonly IReadOnlyDictionary<string, FilterField> Fields = new Dictionary<string, FilterField>
    {
        ["tags"] = new("COALESCE(s.\"Tags\", '{}')", "text", FilterOperators.Array, FilterValue.String, IsArray: true)
    };

    private static FilterRule Rule(object rule) => JsonSerializer.Deserialize<FilterRule>(JsonSerializer.Serialize(rule))!;

    [Fact]
    public void In_ShouldCompileToArrayOverlap()
    {
        var filter = new FilterSqlCompiler(Fields).Build(Rule(new { op = "in", field = "tags", values = new[] { "a", "b" } }));

        filter.Sql.Should().Be("""(COALESCE(s."Tags", '{}') && ARRAY[@filter_0::text, @filter_1::text]::text[])""");
    }

    [Fact]
    public void NotIn_ShouldCompileToNegatedArrayOverlap()
    {
        var filter = new FilterSqlCompiler(Fields).Build(Rule(new { op = "notIn", field = "tags", values = new[] { "a" } }));

        filter.Sql.Should().Be("""(NOT (COALESCE(s."Tags", '{}') && ARRAY[@filter_0::text]::text[]))""");
    }

    [Fact]
    public void In_WithFunctionParameter_ShouldKeepValuesOutOfTheSql()
    {
        var filter = new FilterSqlCompiler(Fields, parameterPrefixer: FilterSqlCompiler.FunctionParameter)
            .Build(Rule(new { op = "in", field = "tags", values = new[] { "'; DROP TABLE x; --" } }));
        var parameters = JsonNode.Parse(filter.ToJsonParameters())!.AsObject();

        filter.Sql.Should().Be("""(COALESCE(s."Tags", '{}') && ARRAY[($1 ->> 'filter_0')::text]::text[])""");
        parameters["filter_0"]!.GetValue<string>().Should().Be("'; DROP TABLE x; --");
    }

    [Theory]
    [InlineData("in", "FALSE")]
    [InlineData("notIn", "TRUE")]
    public void In_WithNoValues_ShouldCompileToConstant(string op, string expected)
    {
        var filter = new FilterSqlCompiler(Fields).Build(Rule(new { op, field = "tags", values = Array.Empty<string>() }));

        filter.Sql.Should().Be(expected);
    }

    [Fact]
    public void Eq_ShouldNotBeAllowed()
    {
        var act = () => new FilterSqlCompiler(Fields).Build(Rule(new { op = "eq", field = "tags", value = "a" }));

        act.Should().Throw<ArgumentException>().WithMessage("Operator 'eq' is not supported for field 'tags'.");
    }
}
