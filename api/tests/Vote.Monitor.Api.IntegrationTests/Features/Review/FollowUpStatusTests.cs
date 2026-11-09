using System.Net;
using System.Net.Http.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Review;

using static ApiTesting;

public class FollowUpStatusTests : BaseApiTestFixture
{
    public record StatusResponse(Guid Id, string Name, string? Description, double Sequence, bool IsDefault, string? Color,
        string? Icon);

    public record EntryResponse(Guid SubmissionId, string FollowUpStatus, Guid? FollowUpStatusId, string? FollowUpStatusName,
        string? FollowUpStatusColor);

    public record ViewResponse(string FollowUpStatus, Guid? FollowUpStatusId, string? FollowUpStatusName);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    public record HistoryEntryResponse(string Kind, string? FollowUpStatus);

    /// <summary>Alfa leads a coalition with Beta; Alice observes for Alfa and submitted the common form.</summary>
    private (ScenarioData Data, Guid SubmissionId) Scenario()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta)
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta], form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi))))
            .Please();
        return (data, data.ElectionRound.Coalition.GetSubmissionId("Common", ScenarioObserver.Alice, ScenarioPollingStation.Iasi));
    }

    private static string StatusesUri(Guid er) => $"/api/election-rounds/{er}/follow-up-statuses";
    private static string FollowUpUri(Guid er, Guid id) => $"/api/election-rounds/{er}/follow-ups/FormSubmission/{id}";

    private static StatusResponse[] Statuses(HttpClient client, Guid er) => client.GetResponse<StatusResponse[]>(StatusesUri(er));

    private static EntryResponse Entry(HttpClient client, Guid er, Guid id, string dataSource = "Ngo", object? filter = null) =>
        client.PostWithResponse<PagedResponse<EntryResponse>>($"/api/election-rounds/{er}/form-submissions:byEntryV2",
            new { dataSource, filter, pageSize = 100 }).Items.Single(x => x.SubmissionId == id);

    [Test]
    public void EveryNgoStartsWithThreeStatuses_NotApplicableFirstAndDefault()
    {
        var (data, submissionId) = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa).Admin;

        Statuses(alfa, data.ElectionRoundId).Select(x => (x.Name, x.IsDefault, x.Color, x.Icon)).Should().Equal(
            ("Not applicable", true, "gray", "circle"), ("Needs follow-up", false, "yellow", "alert"),
            ("Resolved", false, "green", "check"));
        Entry(alfa, data.ElectionRoundId, submissionId).FollowUpStatusName.Should().Be("Not applicable");
    }

    [Test]
    public void EachNgoKeepsItsOwnFollowUp_AndTheOwnersSeededStatusesKeepTheRecordsFixedValue()
    {
        // Bob observes for Beta, a coalition member; Alfa leads the coalition and sees Bob's submission too.
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta], form => form
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))))
            .Please();
        var submissionId = data.ElectionRound.Coalition.GetSubmissionId("Common", ScenarioObserver.Bob, ScenarioPollingStation.Iasi);
        var er = data.ElectionRoundId;
        var alfa = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var beta = data.NgoByName(ScenarioNgos.Beta).Admin;
        var calling = alfa.PostWithResponse<StatusResponse>(StatusesUri(er),
            new { name = "Calling the observer", color = "orange", icon = "phone" });
        var alfaResolved = Statuses(alfa, er).Single(x => x.Name == "Resolved");

        alfa.PutWithoutResponse(FollowUpUri(er, submissionId), new { followUpStatusId = calling.Id });

        Entry(alfa, er, submissionId, "Coalition").Should().Match<EntryResponse>(x =>
            x.FollowUpStatusName == "Calling the observer" && x.FollowUpStatusColor == "orange");
        Entry(beta, er, submissionId).FollowUpStatusName.Should().Be("Not applicable");

        // Alfa doesn't own the record: even its seeded statuses leave the record's fixed value alone.
        alfa.PutWithoutResponse(FollowUpUri(er, submissionId), new { followUpStatusId = alfaResolved.Id });
        Entry(beta, er, submissionId).FollowUpStatus.Should().Be("NotApplicable");

        // Beta owns it: its seeded statuses move the fixed value too.
        var betaNeeds = Statuses(beta, er).Single(x => x.Name == "Needs follow-up");
        beta.PutWithoutResponse(FollowUpUri(er, submissionId), new { followUpStatusId = betaNeeds.Id });
        beta.GetResponse<ViewResponse>($"/api/election-rounds/{er}/form-submissions/{submissionId}:v2")
            .Should().Match<ViewResponse>(x => x.FollowUpStatus == "NeedsFollowUp" && x.FollowUpStatusName == "Needs follow-up");
        Entry(alfa, er, submissionId, "Coalition").FollowUpStatusName.Should().Be("Resolved");

        // Filtering by the NGO's own status id.
        var filter = new { field = "followUpStatus", op = "eq", value = alfaResolved.Id };
        Entry(alfa, er, submissionId, "Coalition", filter).FollowUpStatusId.Should().Be(alfaResolved.Id);

        // Another NGO's status can't be used.
        alfa.PutAsJsonAsync(FollowUpUri(er, submissionId), new { followUpStatusId = betaNeeds.Id }).GetAwaiter().GetResult()
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);

        alfa.GetResponse<HistoryEntryResponse[]>($"/api/election-rounds/{er}/reviews/FormSubmission/{submissionId}/history")
            .Where(x => x.Kind == "FollowUpStatusChanged").Select(x => x.FollowUpStatus)
            .Should().Equal("Resolved", "Calling the observer");
    }

    [Test]
    public void TheOldStatusEndpointMovesTheCallersOwnFollowUp()
    {
        var (data, submissionId) = Scenario();
        var er = data.ElectionRoundId;
        var alfa = data.NgoByName(ScenarioNgos.Alfa).Admin;

        alfa.PutWithoutResponse($"/api/election-rounds/{er}/form-submissions/{submissionId}:status",
            new { followUpStatus = "NeedsFollowUp" });

        Entry(alfa, er, submissionId).Should().Match<EntryResponse>(x =>
            x.FollowUpStatus == "NeedsFollowUp" && x.FollowUpStatusName == "Needs follow-up");
    }

    [Test]
    public void StatusesAreManagedLikeStages_DefaultAndInUseRules()
    {
        var (data, submissionId) = Scenario();
        var er = data.ElectionRoundId;
        var alfa = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var statuses = Statuses(alfa, er);
        var notApplicable = statuses.Single(x => x.IsDefault);
        var needs = statuses.Single(x => x.Name == "Needs follow-up");
        var police = alfa.PostWithResponse<StatusResponse>(StatusesUri(er), new { name = "Called the police", description = "Reported to the police" });

        var duplicate = alfa.PostAsJsonAsync(StatusesUri(er), new { name = "resolved" }).GetAwaiter().GetResult();
        var badIcon = alfa.PostAsJsonAsync(StatusesUri(er), new { name = "Odd", icon = "rocket" }).GetAwaiter().GetResult();
        var deleteDefault = alfa.DeleteAsync($"{StatusesUri(er)}/{notApplicable.Id}").GetAwaiter().GetResult();
        duplicate.StatusCode.Should().Be(HttpStatusCode.Conflict);
        badIcon.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        deleteDefault.StatusCode.Should().Be(HttpStatusCode.Conflict);
        police.Sequence.Should().BeGreaterThan(statuses.Max(x => x.Sequence));

        // A new default doesn't move records still on the old one.
        var makeDefault = alfa.PostAsync($"{StatusesUri(er)}/{needs.Id}:default", null).GetAwaiter().GetResult();
        makeDefault.IsSuccessStatusCode.Should().BeTrue(makeDefault.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        Entry(alfa, er, submissionId).FollowUpStatusName.Should().Be("Not applicable");
        Statuses(alfa, er).Single(x => x.IsDefault).Id.Should().Be(needs.Id);

        // A status in use needs somewhere to move its records.
        alfa.PutWithoutResponse(FollowUpUri(er, submissionId), new { followUpStatusId = police.Id });
        alfa.DeleteAsync($"{StatusesUri(er)}/{police.Id}").GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.Conflict);
        alfa.DeleteAsync($"{StatusesUri(er)}/{police.Id}?moveTo={needs.Id}").GetAwaiter().GetResult()
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        Entry(alfa, er, submissionId).FollowUpStatusName.Should().Be("Needs follow-up");
    }

    public record AggregateResponse(AggregateCounts QuickReportsAggregate);

    public record AggregateCounts(Dictionary<string, int> FollowUpStatusHistogram);

    public record AdminRowResponse(Guid Id, string FollowUpStatus, string? FollowUpStatusName);

    public record ByObserverResponse(Guid MonitoringObserverId, FollowUpCountResponse[] FollowUps);

    public record FollowUpCountResponse(Guid Id, string Name, int Count);

    [Test]
    public void QuickReportChartsPlatformListsAndByObserverUseTheNgosOwnStatuses()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi))
            .Please();
        var er = data.ElectionRoundId;
        var alfa = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var quickReportId = data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var police = alfa.PostWithResponse<StatusResponse>(StatusesUri(er), new { name = "Called the police" });
        alfa.PutWithoutResponse($"/api/election-rounds/{er}/follow-ups/QuickReport/{quickReportId}", new { followUpStatusId = police.Id });

        // The chart counts by the NGO's statuses, every one of them listed.
        alfa.PostWithResponse<AggregateResponse>($"/api/election-rounds/{er}/quick-reports:aggregated", new { })
            .QuickReportsAggregate.FollowUpStatusHistogram.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                ["Not applicable"] = 0, ["Needs follow-up"] = 0, ["Resolved"] = 0, ["Called the police"] = 1
            });

        // Platform admins see the status of the NGO that owns the record.
        var row = data.PlatformAdmin.GetResponse<PagedResponse<AdminRowResponse>>($"/api/election-rounds/{er}/quick-reports:all")
            .Items.Single(x => x.Id == quickReportId);
        row.FollowUpStatusName.Should().Be("Called the police");
        data.PlatformAdmin.GetResponse<PagedResponse<AdminRowResponse>>($"/api/election-rounds/{er}/incident-reports:all").Items.Should().BeEmpty();
        data.PlatformAdmin.GetResponse<PagedResponse<AdminRowResponse>>($"/api/election-rounds/{er}/citizen-reports:all").Items.Should().BeEmpty();

        // By observer: counts per status (none here: Alice sent no incident report).
        alfa.PostWithResponse<PagedResponse<ByObserverResponse>>($"/api/election-rounds/{er}/incident-reports:byObserverV2", new { dataSource = "Ngo" })
            .Items.Should().ContainSingle().Which.FollowUps.Should().BeEmpty();
    }
}
