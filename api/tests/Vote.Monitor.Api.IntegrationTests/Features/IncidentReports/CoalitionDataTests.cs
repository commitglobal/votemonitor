using System.Net;
using System.Net.Http.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Api.IntegrationTests.Features.IncidentReports;

using static ApiTesting;

/// <summary>
/// A coalition leader reads incident reports as its own NGO's or as the coalition's (`dataSource`), in the list, the
/// insights and the answers aggregate, the same way as form submissions.
/// </summary>
public class CoalitionDataTests : BaseApiTestFixture
{
    public record EntryResponse(Guid IncidentReportId);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    public record InsightsResponse(int Total);

    public record AggregateInner(int SubmissionCount);

    public record AggregateResponse(AggregateInner SubmissionsAggregate);

    [Test]
    public void TheLeaderSeesMembersIncidentReportsAsTheCoalitionsOnly()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta]))
            .Please();
        var er = data.ElectionRoundId;
        var leader = data.NgoByName(ScenarioNgos.Alfa).Admin;

        // The leader's incident form, shared with the member.
        var incidentForm = Dummy.Form("INC");
        incidentForm.FormType = FormType.IncidentReporting;
        var form = leader.PostWithResponse<CreateFormRequest>($"/api/election-rounds/{er}/forms", incidentForm);
        leader.PostWithoutResponse($"/api/election-rounds/{er}/forms/{form.Id}:publish");
        leader.PutWithoutResponse(
            $"/api/election-rounds/{er}/coalitions/{data.ElectionRound.CoalitionId}/forms/{form.Id}:access",
            new { NgoMembersIds = new[] { data.NgoIdByName(ScenarioNgos.Beta) } });

        Guid Report(ScenarioObserver observer)
        {
            var id = Guid.NewGuid();
            data.ObserverByName(observer)
                .PostAsJsonAsync($"/api/election-rounds/{er}/incident-reports",
                    new { id, formId = form.Id, locationType = "NotRelatedToAPollingStation", locationDescription = "At the town hall" })
                .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.OK);
            return id;
        }

        var alices = Report(ScenarioObserver.Alice);
        var bobs = Report(ScenarioObserver.Bob);

        List<Guid> Entries(string dataSource) =>
            leader.PostWithResponse<PagedResponse<EntryResponse>>($"/api/election-rounds/{er}/incident-reports:byEntryV2",
                new { dataSource }).Items.Select(x => x.IncidentReportId).ToList();

        int Total(string dataSource) =>
            leader.PostWithResponse<InsightsResponse>($"/api/election-rounds/{er}/reviews/IncidentReport:insights",
                new { dataSource, metric = "records", timeZone = "UTC" }).Total;

        int Aggregated(string dataSource) =>
            leader.PostWithResponse<AggregateResponse>(
                    $"/api/election-rounds/{er}/incident-reports/forms/{form.Id}:aggregated-submissionsV2",
                    new { dataSource })
                .SubmissionsAggregate.SubmissionCount;

        Entries("Ngo").Should().BeEquivalentTo([alices]);
        Entries("Coalition").Should().BeEquivalentTo([alices, bobs]);
        Total("Ngo").Should().Be(1);
        Total("Coalition").Should().Be(2);
        Aggregated("Ngo").Should().Be(1);
        Aggregated("Coalition").Should().Be(2);
    }
}
