using System.Text.Json;
using System.Text.Json.Nodes;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;

namespace Vote.Monitor.Core.UnitTests.RulesEngine;

public class FilterRulesTests
{
    private static readonly IReadOnlyDictionary<string, FilterField> Fields = new Dictionary<string, FilterField>
    {
        ["level1"] = new("s.\"Level1\"", "text", FilterOperators.Set, FilterValue.String),
        ["isCompleted"] = new("s.\"IsCompleted\"", "boolean", FilterOperators.Boolean, FilterValue.Boolean)
    };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Eq_WithoutValue_ShouldReturnNull(string? value)
    {
        FilterRules.Eq("level1", value).Should().BeNull();
    }

    [Fact]
    public void Eq_WithValue_ShouldReturnEqRule()
    {
        var rule = FilterRules.Eq("isCompleted", true);

        rule.Should().BeOfType<EqFilter>();
        var eq = (EqFilter)rule!;
        eq.Field.Should().Be("isCompleted");
        eq.Value!.Value.GetBoolean().Should().BeTrue();
    }

    [Fact]
    public void And_WithoutRules_ShouldReturnNull()
    {
        FilterRules.And(null, null).Should().BeNull();
    }

    [Fact]
    public void And_WithOneRule_ShouldReturnThatRule()
    {
        var rule = FilterRules.Eq("level1", "North");

        FilterRules.And(null, rule, null).Should().BeSameAs(rule);
    }

    [Fact]
    public void And_WithSeveralRules_ShouldCompileToAllOfThem()
    {
        var requestFilter = JsonSerializer.Deserialize<FilterRule>("""{ "op": "eq", "field": "isCompleted", "value": true }""");

        var rule = FilterRules.And(requestFilter, FilterRules.Eq("level1", "North"), FilterRules.Eq("level1", null));
        var filter = new FilterSqlCompiler(Fields, parameterPrefixer: FilterSqlCompiler.FunctionParameter).Build(rule);
        var parameters = JsonNode.Parse(filter.ToJsonParameters())!.AsObject();

        rule.Should().BeOfType<AndFilter>().Which.Values.Should().HaveCount(2);
        filter.Sql.Should().StartWith("(").And.Contain(" AND ").And.Contain("s.\"IsCompleted\"").And.Contain("s.\"Level1\"");
        parameters.Select(p => p.Value!.ToJsonString()).Should().BeEquivalentTo("true", "\"North\"");
    }
}
