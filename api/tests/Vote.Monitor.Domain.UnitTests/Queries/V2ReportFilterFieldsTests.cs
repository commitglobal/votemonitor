using System.Text.Json;
using System.Text.Json.Nodes;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.Queries;

namespace Vote.Monitor.Domain.UnitTests.Queries;

public class V2ReportFilterFieldsTests
{
    private static FilterRule Rule(object rule) => JsonSerializer.Deserialize<FilterRule>(JsonSerializer.Serialize(rule))!;

    [Theory]
    [InlineData("PollingStation", "PollingStation")]
    [InlineData("OtherPollingStation", "OtherPollingStation")]
    [InlineData("NotRelatedToAPollingStation", "NotRelatedToAPollingStation")]
    [InlineData("OtherLocation", "NotRelatedToAPollingStation")]
    public void IncidentReportLocationType_ShouldAcceptValuesAndLegacyOtherLocation(string value, string expected)
    {
        var filter = new FilterSqlCompiler(V2ReportFilterFields.IncidentReports, parameterPrefixer: FilterSqlCompiler.FunctionParameter)
            .Build(Rule(new { op = "eq", field = "locationType", value }));
        var parameters = JsonNode.Parse(filter.ToJsonParameters())!.AsObject();

        parameters["filter_0"]!.GetValue<string>().Should().Be(expected);
    }

    [Fact]
    public void IncidentReportLocationType_ShouldRejectUnknownValue()
    {
        var act = () => new FilterSqlCompiler(V2ReportFilterFields.IncidentReports)
            .Build(Rule(new { op = "eq", field = "locationType", value = "Somewhere" }));

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("FormSubmissions")]
    [InlineData("IncidentReports")]
    public void Tags_ShouldBeFilterable(string report)
    {
        var fields = report == "FormSubmissions" ? V2ReportFilterFields.FormSubmissions : V2ReportFilterFields.IncidentReports;

        var filter = new FilterSqlCompiler(fields).Build(Rule(new { op = "in", field = "tags", values = new[] { "a" } }));

        filter.Sql.Should().Be("""(COALESCE(s."Tags", '{}') && ARRAY[@filter_0::text]::text[])""");
    }
}
