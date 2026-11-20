using System.Net;
using System.Net.Http.Json;
using Module.Answers.Requests;
using Module.Forms.Requests;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Api.IntegrationTests.Features.Review;

using static ApiTesting;

public class InsightsTests : BaseApiTestFixture
{
    private const string Sam = "Sam";

    public record StageResponse(Guid Id, string Name);

    public record BucketResponse(string? Key, string? Label, long Value);

    public record BarResponse(string? Key, string? Label, long Value, BucketResponse[]? Parts);

    public record InsightsResponse(string? XAxis, string? GroupBy, string Metric, long Total, BarResponse[] Bars,
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
    public void WithoutAnAxisCountsTheTotalOnly_DistinctPollingStationsAndObserversToo()
    {
        var (data, _, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        var stations = admin.PostWithResponse<InsightsResponse>(Uri(data.ElectionRoundId),
            new { dataSource = "Coalition", metric = "pollingStations" });
        var observers = admin.PostWithResponse<InsightsResponse>(Uri(data.ElectionRoundId),
            new { dataSource = "Coalition", metric = "observers" });
        var own = admin.PostWithResponse<InsightsResponse>(Uri(data.ElectionRoundId), new { dataSource = "Ngo" });

        stations.XAxis.Should().BeNull();
        stations.Total.Should().Be(2);
        stations.Bars.Should().BeEmpty();
        stations.Groups.Should().BeNull();
        observers.Total.Should().Be(2);
        own.Total.Should().Be(1);
        admin.PostAsJsonAsync(Uri(data.ElectionRoundId), new { groupBy = "stage" }).GetAwaiter().GetResult()
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

    public record LocationResponse(Guid Id, string Level1);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    private static TranslatedString Text(string text) => new() { { "EN", text } };

    /// <summary>Alfa monitors Alice and Bob; Alice sent a quick report.</summary>
    private ScenarioData UnansweredScenario() =>
        ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioObserver.Bob))
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi))
            .Please();

    /// <summary>Creates and publishes a form of two text questions; returns its id and the questions' ids.</summary>
    private static (Guid FormId, Guid Q1, Guid Q2) CreateTwoQuestionForm(HttpClient admin, Guid electionRoundId,
        FormType formType)
    {
        var form = admin.PostWithResponse<CreateFormRequest>($"/api/election-rounds/{electionRoundId}/forms",
            new CreateFormRequest
            {
                Code = formType.Name,
                DefaultLanguage = "EN",
                Languages = ["EN"],
                Name = Text(formType.Name),
                Description = Text(formType.Name),
                FormType = formType,
                Questions =
                [
                    new TextQuestionRequest { Id = Guid.NewGuid(), Code = "Q1", Text = Text("Q1") },
                    new TextQuestionRequest { Id = Guid.NewGuid(), Code = "Q2", Text = Text("Q2") }
                ]
            });
        admin.PostWithoutResponse($"/api/election-rounds/{electionRoundId}/forms/{form.Id}:publish");
        return (form.Id, form.Questions.Single(x => x.Code == "Q1").Id, form.Questions.Single(x => x.Code == "Q2").Id);
    }

    /// <summary>Answers to Q1, and to Q2 too when <paramref name="both"/>.</summary>
    private static List<BaseAnswerRequest> Answers((Guid FormId, Guid Q1, Guid Q2) form, bool both)
    {
        List<BaseAnswerRequest> answers = [new TextAnswerRequest { QuestionId = form.Q1, Text = "one" }];
        if (both)
        {
            answers.Add(new TextAnswerRequest { QuestionId = form.Q2, Text = "two" });
        }

        return answers;
    }

    private static InsightsResponse Insights(HttpClient admin, Guid electionRoundId, string recordType, object body) =>
        admin.PostWithResponse<InsightsResponse>(Uri(electionRoundId, recordType), body);

    /// <summary>Unanswered questions summed, and the records with and without them.</summary>
    private static void CountsUnansweredQuestions(HttpClient admin, Guid electionRoundId, string recordType)
    {
        var sum = Insights(admin, electionRoundId, recordType, new { metric = "unansweredQuestions" });
        sum.XAxis.Should().BeNull();
        sum.Total.Should().Be(1);
        sum.Bars.Should().BeEmpty();
        sum.Groups.Should().BeNull();

        var byAxis = Insights(admin, electionRoundId, recordType, new { xAxis = "hasUnansweredQuestions" });
        byAxis.Total.Should().Be(2);
        byAxis.Bars.Select(x => (x.Key, x.Value)).Should().BeEquivalentTo([("true", 1L), ("false", 1L)]);

        var summedByAxis = Insights(admin, electionRoundId, recordType,
            new { xAxis = "hasUnansweredQuestions", metric = "unansweredQuestions" });
        summedByAxis.Bars.Select(x => (x.Key, x.Value)).Should().BeEquivalentTo([("true", 1L), ("false", 0L)]);
    }

    [Test]
    public void FormSubmissions_MeasureUnansweredQuestions()
    {
        var data = UnansweredScenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var form = CreateTwoQuestionForm(admin, er, FormType.Opening);

        void Submit(ScenarioObserver observer, bool both) =>
            data.ObserverByName(observer).PostWithoutResponse($"/api/election-rounds/{er}/form-submissions",
                new FormSubmissionRequest
                {
                    PollingStationId = data.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi),
                    FormId = form.FormId,
                    Answers = Answers(form, both)
                });

        Submit(ScenarioObserver.Alice, both: false);
        Submit(ScenarioObserver.Bob, both: true);

        CountsUnansweredQuestions(admin, er, "FormSubmission");
    }

    [Test]
    public void IncidentReports_MeasureUnansweredQuestions()
    {
        var data = UnansweredScenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var form = CreateTwoQuestionForm(admin, er, FormType.IncidentReporting);

        void Report(ScenarioObserver observer, bool both) =>
            data.ObserverByName(observer).PostAsJsonAsync($"/api/election-rounds/{er}/incident-reports",
                    new
                    {
                        id = Guid.NewGuid(),
                        formId = form.FormId,
                        locationType = "NotRelatedToAPollingStation",
                        locationDescription = "At the town hall",
                        answers = Answers(form, both)
                    })
                .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.OK);

        Report(ScenarioObserver.Alice, both: false);
        Report(ScenarioObserver.Bob, both: true);

        CountsUnansweredQuestions(admin, er, "IncidentReport");
    }

    [Test]
    public void CitizenReports_MeasureUnansweredQuestionsAndTheLocationsReportedFrom()
    {
        var data = UnansweredScenario();
        var er = data.ElectionRoundId;
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:enableCitizenReporting",
            new { ngoId = alfa.NgoId });
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}/locations",
            new
            {
                locations = new[] { "Iasi", "Cluj", "Arad" }.Select((level1, i) => new
                {
                    level1, level2 = "", level3 = "", level4 = "", level5 = "", displayOrder = i,
                    tags = new Dictionary<string, string>()
                })
            });
        var locations = data.PlatformAdmin
            .GetResponse<PagedResponse<LocationResponse>>($"/api/election-rounds/{er}/locations:list?pageSize=100")
            .Items.ToDictionary(x => x.Level1, x => x.Id);
        var form = CreateTwoQuestionForm(alfa.Admin, er, FormType.CitizenReporting);
        var citizen = CreateClient();

        void Report(string location, bool both) =>
            citizen.PostAsJsonAsync($"/api/election-rounds/{er}/citizen-reports",
                    new
                    {
                        citizenReportId = Guid.NewGuid(),
                        formId = form.FormId,
                        locationId = locations[location],
                        answers = Answers(form, both)
                    })
                .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.OK);

        Report("Iasi", both: false);
        Report("Iasi", both: true);

        CountsUnansweredQuestions(alfa.Admin, er, "CitizenReport");

        Report("Cluj", both: true);
        Insights(alfa.Admin, er, "CitizenReport", new { metric = "locations" }).Total.Should().Be(2);
        alfa.Admin.PostAsJsonAsync(Uri(er), new { metric = "locations" }).GetAwaiter().GetResult()
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public void QuickReportsHaveNoUnansweredQuestions()
    {
        var data = UnansweredScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        HttpStatusCode Post(object body) =>
            admin.PostAsJsonAsync(Uri(data.ElectionRoundId, "QuickReport"), body).GetAwaiter().GetResult().StatusCode;

        Post(new { metric = "unansweredQuestions" }).Should().Be(HttpStatusCode.BadRequest);
        Post(new { xAxis = "hasUnansweredQuestions" }).Should().Be(HttpStatusCode.BadRequest);
        Post(new { metric = "locations" }).Should().Be(HttpStatusCode.BadRequest);
    }

    [TestCase("FormSubmission", new[] { "records", "flaggedAnswers", "mediaFiles", "notes", "comments", "observers", "pollingStations", "unansweredQuestions" })]
    [TestCase("IncidentReport", new[] { "records", "flaggedAnswers", "mediaFiles", "notes", "comments", "observers", "pollingStations", "unansweredQuestions" })]
    [TestCase("CitizenReport", new[] { "records", "flaggedAnswers", "mediaFiles", "notes", "comments", "unansweredQuestions", "locations" })]
    [TestCase("QuickReport", new[] { "records", "mediaFiles", "comments", "observers", "pollingStations" })]
    public void WithoutAnAxisEveryRecordTypeCountsTheTotalOfEachOfItsMetrics(string recordType, string[] metrics)
    {
        var data = UnansweredScenario();
        var er = data.ElectionRoundId;
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        if (recordType == "CitizenReport")
        {
            data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:enableCitizenReporting",
                new { ngoId = alfa.NgoId });
        }

        foreach (var metric in metrics)
        {
            var insights = Insights(alfa.Admin, er, recordType, new { dataSource = "Ngo", metric });
            insights.Metric.Should().Be(metric);
            insights.XAxis.Should().BeNull();
            insights.GroupBy.Should().BeNull();
            insights.Bars.Should().BeEmpty();
            insights.Groups.Should().BeNull();
            if (recordType == "QuickReport" && metric is "records" or "observers" or "pollingStations")
            {
                insights.Total.Should().Be(1, metric);
            }
        }
    }
}
