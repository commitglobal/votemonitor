using System.Text.Json;
using System.Text.Json.Nodes;
using Feature.CitizenReports.GetSubmissionsAggregatedV2;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Queries;

namespace Feature.CitizenReports.UnitTests.Endpoints;

public class GetSubmissionsAggregatedV2RequestTests
{
    private static CompiledFilter Compile(Request request) =>
        new FilterSqlCompiler(V2ReportFilterFields.CitizenReports, parameterPrefixer: FilterSqlCompiler.FunctionParameter)
            .Build(request.ToFilterRule());

    [Fact]
    public void ToFilterRule_WithoutFilterOrQueryParams_ShouldNotFilter()
    {
        Compile(new Request()).Sql.Should().Be("1 = 1");
    }

    [Fact]
    public void ToFilterRule_ShouldApplyQueryParamsAlongsideFilter()
    {
        var locationId = Guid.NewGuid();
        var request = new Request
        {
            Filter = JsonSerializer.Deserialize<FilterRule>("""{ "op": "eq", "field": "hasComments", "value": true }"""),
            Level1Filter = "North",
            Level3Filter = "City",
            LocationId = locationId,
            HasFlaggedAnswers = true,
            FollowUpStatus = CitizenReportFollowUpStatus.Resolved,
            HasNotes = false,
            HasAttachments = true,
            QuestionsAnswered = QuestionsAnsweredFilter.Some
        };

        var filter = Compile(request);
        var parameters = JsonNode.Parse(filter.ToJsonParameters())!.AsObject()
            .Select(p => p.Value!.ToJsonString());

        foreach (var column in new[]
                 {
                     "s.\"HasComments\"", "s.\"Level1\"", "s.\"Level3\"", "s.\"LocationId\"",
                     "(s.\"NumberOfFlaggedAnswers\" > 0)", "s.\"FollowUpStatus\"", "s.\"HasNotes\"",
                     "s.\"HasAttachments\"", "s.\"QuestionsAnswered\""
                 })
        {
            filter.Sql.Should().Contain(column);
        }

        parameters.Should().BeEquivalentTo(
            "true", "\"North\"", "\"City\"", $"\"{locationId}\"", "true", "\"Resolved\"", "false", "true", "\"Some\"");
    }
}
