using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Feature.IncidentReports.GetSubmissionsAggregatedV2;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Queries;
using Xunit;

namespace Feature.IncidentReports.UnitTests.Endpoints;

public class GetSubmissionsAggregatedV2RequestTests
{
    private static CompiledFilter Compile(Request request) =>
        new FilterSqlCompiler(V2ReportFilterFields.IncidentReports, parameterPrefixer: FilterSqlCompiler.FunctionParameter)
            .Build(request.ToFilterRule());

    [Fact]
    public void ToFilterRule_WithoutFilterOrQueryParams_ShouldNotFilter()
    {
        Compile(new Request()).Sql.Should().Be("1 = 1");
    }

    [Fact]
    public void ToFilterRule_ShouldApplyQueryParamsAlongsideFilter()
    {
        var pollingStationId = Guid.NewGuid();
        var request = new Request
        {
            Filter = JsonSerializer.Deserialize<FilterRule>("""{ "op": "eq", "field": "hasComments", "value": true }"""),
            Level1Filter = "North",
            Level2Filter = " ",
            PollingStationNumberFilter = "12",
            PollingStationId = pollingStationId,
            HasFlaggedAnswers = false,
            FollowUpStatusFilter = IncidentReportFollowUpStatus.NeedsFollowUp,
            LocationTypeFilter = IncidentReportLocationType.PollingStation,
            HasNotes = true,
            HasAttachments = false,
            QuestionsAnswered = QuestionsAnsweredFilter.All,
            IsCompletedFilter = true
        };

        var filter = Compile(request);
        var parameters = JsonNode.Parse(filter.ToJsonParameters())!.AsObject()
            .Select(p => p.Value!.ToJsonString());

        foreach (var column in new[]
                 {
                     "s.\"HasComments\"", "s.\"Level1\"", "s.\"Number\"", "s.\"PollingStationId\"",
                     "(s.\"NumberOfFlaggedAnswers\" > 0)", "s.\"FollowUpStatus\"", "s.\"LocationType\"", "s.\"HasNotes\"",
                     "s.\"HasAttachments\"", "s.\"QuestionsAnswered\"", "s.\"IsCompleted\""
                 })
        {
            filter.Sql.Should().Contain(column);
        }

        // blank query params add no condition
        filter.Sql.Should().NotContain("s.\"Level2\"");
        parameters.Should().BeEquivalentTo(
            "true", "\"North\"", "\"12\"", $"\"{pollingStationId}\"", "false", "\"NeedsFollowUp\"", "\"PollingStation\"",
            "true", "false", "\"All\"", "true");
    }
}
