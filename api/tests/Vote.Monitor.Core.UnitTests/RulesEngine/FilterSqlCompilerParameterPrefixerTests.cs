using System.Text.Json;
using System.Text.Json.Nodes;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;

namespace Vote.Monitor.Core.UnitTests.RulesEngine;

public class FilterSqlCompilerParameterPrefixerTests
{
    private static readonly IReadOnlyDictionary<string, FilterField> Fields = new Dictionary<string, FilterField>
    {
        ["submittedAt"] = new("s.\"TimeSubmitted\"", "timestamptz", FilterOperators.Comparison, FilterValue.UtcDateTimeOffset),
        ["level1"] = new("s.\"Level1\"", "text", FilterOperators.Text, FilterValue.String)
    };

    private static FilterRule Rule(object rule) => JsonSerializer.Deserialize<FilterRule>(JsonSerializer.Serialize(rule))!;

    private static readonly FilterRule SubmittedBetween =
        Rule(new { op = "between", field = "submittedAt", from = "2024-06-01T00:00:00Z", to = "2024-06-02T00:00:00Z" });

    [Fact]
    public void Build_WithoutPrefixer_ShouldReferenceDapperParameters()
    {
        var filter = new FilterSqlCompiler(Fields).Build(SubmittedBetween);

        filter.Sql.Should().Be("""(s."TimeSubmitted" >= @filter_0::timestamptz AND s."TimeSubmitted" <= @filter_1::timestamptz)""");
    }

    [Fact]
    public void Build_WithFunctionParameter_ShouldReferenceJsonbParameters()
    {
        var filter = new FilterSqlCompiler(Fields, parameterPrefixer: FilterSqlCompiler.FunctionParameter)
            .Build(SubmittedBetween);

        filter.Sql.Should().Be(
            """(s."TimeSubmitted" >= ($1 ->> 'filter_0')::timestamptz AND s."TimeSubmitted" <= ($1 ->> 'filter_1')::timestamptz)""");
    }

    [Fact]
    public void ToJsonParameters_ShouldKeepValuesOutOfTheSql()
    {
        const string payload = "TRUE) OR TRUE; DROP TABLE \"Forms\"; --";

        var filter = new FilterSqlCompiler(Fields, parameterPrefixer: FilterSqlCompiler.FunctionParameter)
            .Build(Rule(new { op = "contains", field = "level1", value = payload }));
        var parameters = JsonNode.Parse(filter.ToJsonParameters())!.AsObject();

        filter.Sql.Should().Be("""s."Level1" LIKE ($1 ->> 'filter_0') ESCAPE '\'""");
        filter.Sql.Should().NotContain("DROP");
        parameters["filter_0"]!.GetValue<string>().Should().Be($"%{payload}%");
    }
}
