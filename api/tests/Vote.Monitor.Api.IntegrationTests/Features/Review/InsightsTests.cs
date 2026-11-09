using System.Net;
using System.Net.Http.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Review;

using static ApiTesting;

public class InsightsTests : BaseApiTestFixture
{
    private const string Sam = "Sam";

    public record StageResponse(Guid Id, string Name);

    public record BucketResponse(string? Key, string? Label, long Value);

    public record BarResponse(string? Key, string? Label, long Value, BucketResponse[]? Parts);

    public record InsightsResponse(string XAxis, string? GroupBy, string Metric, long Total, BarResponse[] Bars,
        BucketResponse[]? Groups);

    /// <summary>Alfa leads a coalition with Beta; Alice (Alfa) and Bob (Beta) each submitted the common form.</summary>
    private (ScenarioData Data, Guid AliceSubmissionId, Guid BobSubmissionId) CoalitionScenario()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithStaff(Sam))
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta], form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))))
            .Please();

        var coalition = data.ElectionRound.Coalition;
        return (data,
            coalition.GetSubmissionId("Common", ScenarioObserver.Alice, ScenarioPollingStation.Cluj),
            coalition.GetSubmissionId("Common", ScenarioObserver.Bob, ScenarioPollingStation.Iasi));
    }

    private static string Uri(Guid electionRoundId, string recordType = "FormSubmission") =>
        $"/api/election-rounds/{electionRoundId}/reviews/{recordType}:insights";

    private static Guid StageNamed(HttpClient client, Guid electionRoundId, string name) =>
        client.GetResponse<StageResponse[]>($"/api/election-rounds/{electionRoundId}/review-stages")
            .Single(x => x.Name == name).Id;

    [Test]
    public void GroupsTheListsRows_CountingARecordOncePerReviewerButOnceInEveryTotal()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var review = $"/api/election-rounds/{data.ElectionRoundId}/reviews/FormSubmission/{aliceSubmissionId}";
        alfa.Admin.PutWithoutResponse($"{review}:stage", new { stageId = StageNamed(alfa.Admin, data.ElectionRoundId, "Reviewed") });
        alfa.Admin.PutWithoutResponse($"{review}:reviewers", new { userIds = new[] { alfa.AdminId, alfa.StaffIdByName(Sam) } });

        var insights = alfa.Admin.PostWithResponse<InsightsResponse>(Uri(data.ElectionRoundId),
            new { dataSource = "Coalition", xAxis = "stage", groupBy = "reviewer" });

        insights.Total.Should().Be(2);
        insights.Bars.Select(x => (x.Label, x.Value)).Should().BeEquivalentTo([("Reviewed", 1L), ("Not reviewed", 1L)]);
        var reviewed = insights.Bars.Single(x => x.Label == "Reviewed");
        reviewed.Parts!.Select(x => x.Key).Should().BeEquivalentTo([alfa.AdminId.ToString(), alfa.StaffIdByName(Sam).ToString()]);
        reviewed.Parts!.Should().OnlyContain(x => x.Value == 1 && x.Label != null);
        insights.Bars.Single(x => x.Label == "Not reviewed").Parts.Should().ContainSingle(x => x.Key == null && x.Value == 1);
        insights.Groups!.Select(x => x.Value).Sum().Should().Be(3);
        insights.Groups!.Last().Key.Should().BeNull();
    }

    [Test]
    public void MeasuresDistinctObserversAndUsesTheListsFilter()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        alfa.Admin.PutWithoutResponse($"/api/election-rounds/{data.ElectionRoundId}/reviews/FormSubmission/{aliceSubmissionId}:stage",
            new { stageId = StageNamed(alfa.Admin, data.ElectionRoundId, "Reviewed") });

        var observers = alfa.StaffByName(Sam).PostWithResponse<InsightsResponse>(Uri(data.ElectionRoundId),
            new { dataSource = "Coalition", xAxis = "coalitionMember", metric = "observers" });
        var filtered = alfa.Admin.PostWithResponse<InsightsResponse>(Uri(data.ElectionRoundId),
            new
            {
                dataSource = "Coalition", xAxis = "followUpStatus",
                filter = new { op = "in", field = "reviewStatus", values = new[] { "Reviewed" } }
            });

        observers.Total.Should().Be(2);
        observers.Bars.Should().HaveCount(2).And.OnlyContain(x => x.Value == 1 && x.Label != null);
        filtered.Total.Should().Be(1);
        filtered.Bars.Should().ContainSingle(x => x.Label == "Not applicable" && x.Value == 1);
    }

    [Test]
    public void CountsDaysAndHoursInTheGivenTimeZone()
    {
        var (data, _, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        var days = admin.PostWithResponse<InsightsResponse>(Uri(data.ElectionRoundId),
            new { dataSource = "Coalition", xAxis = "submittedDay", groupBy = "level1", timeZone = "Europe/Bucharest" });
        var hours = admin.PostWithResponse<InsightsResponse>(Uri(data.ElectionRoundId),
            new { dataSource = "Coalition", xAxis = "submittedHour", timeZone = "Pacific/Kiritimati" });

        days.Bars.Sum(x => x.Value).Should().Be(2);
        days.Bars.Should().OnlyContain(x => System.Text.RegularExpressions.Regex.IsMatch(x.Key!, @"^\d{4}-\d{2}-\d{2}$"));
        hours.Bars.Should().OnlyContain(x => x.Key!.EndsWith(":00") && x.Key.Contains('T'));
    }

    [Test]
    public void RejectsUnknownAxesMetricsAndTimeZones()
    {
        var (data, _, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        HttpStatusCode Post(object body) =>
            admin.PostAsJsonAsync(Uri(data.ElectionRoundId), body).GetAwaiter().GetResult().StatusCode;

        Post(new { xAxis = "nope" }).Should().Be(HttpStatusCode.BadRequest);
        Post(new { xAxis = "stage", groupBy = "stage" }).Should().Be(HttpStatusCode.BadRequest);
        Post(new { xAxis = "incidentCategory" }).Should().Be(HttpStatusCode.BadRequest);
        Post(new { xAxis = "stage", metric = "votes" }).Should().Be(HttpStatusCode.BadRequest);
        Post(new { xAxis = "stage", timeZone = "Mars/Olympus" }).Should().Be(HttpStatusCode.BadRequest);
        data.ObserverByName(ScenarioObserver.Alice).PostAsJsonAsync(Uri(data.ElectionRoundId), new { xAxis = "stage" })
            .GetAwaiter().GetResult().StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.NotFound);
    }

    [Test]
    public void QuickReportsHaveTheirOwnAxesAndMetrics()
    {
        var (data, _, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        var insights = admin.PostWithResponse<InsightsResponse>(Uri(data.ElectionRoundId, "QuickReport"),
            new { xAxis = "incidentCategory", groupBy = "tag", metric = "mediaFiles" });

        insights.Total.Should().Be(0);
        insights.Bars.Should().BeEmpty();
        admin.PostAsJsonAsync(Uri(data.ElectionRoundId, "QuickReport"), new { xAxis = "form" })
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
