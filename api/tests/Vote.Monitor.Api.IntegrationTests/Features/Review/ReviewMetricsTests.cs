using System.Net;
using System.Net.Http.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Api.IntegrationTests.Features.Review;

using static ApiTesting;

public class ReviewMetricsTests : BaseApiTestFixture
{
    private const string Sam = "Sam";

    public record StageResponse(Guid Id, string Name, string Status, double Sequence, bool IsDefault);

    public record StageCount(Guid StageId, int Count);

    public record SeverityCount(string Severity, int Count);

    public record PlaceMetrics(string? Level1, string? Level2, string? Level3, string? Level4, string? Level5,
        int Total, StageCount[] Stages, SeverityCount[] Severities);

    public record RecordTypeMetrics(string RecordType, int Total, StageCount[] Stages, SeverityCount[] Severities,
        PlaceMetrics[]? Places);

    public record MetricsResponse(RecordTypeMetrics[] RecordTypes);

    private static readonly string[] SeverityOrder = ["Urgent", "High", "Medium", "Low", "None"];

    /// <summary>
    /// Alfa (Alice) submitted form A at Iasi (North › A), Bacau (North › B) and Cluj (South, no level 2), a quick
    /// report at Iasi and an incident report away from any polling station; Beta (Bob) submitted form B at Iasi.
    /// </summary>
    private (ScenarioData Data, Guid Iasi, Guid Bacau, Guid Cluj, Guid BobIasi) Scenario()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithStaff(Sam))
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi, "North", "A")
                .WithPollingStation(ScenarioPollingStation.Bacau, "North", "B")
                .WithPollingStation(ScenarioPollingStation.Cluj, "South")
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithForm("A", form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Bacau)
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Bob)
                    .WithForm("B", form => form
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)))
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi))
            .Please();

        var er = data.ElectionRoundId;
        var alfaAdmin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        var incidentForm = Dummy.Form("INC");
        incidentForm.FormType = FormType.IncidentReporting;
        var form = alfaAdmin.PostWithResponse<CreateFormRequest>($"/api/election-rounds/{er}/forms", incidentForm);
        alfaAdmin.PostWithoutResponse($"/api/election-rounds/{er}/forms/{form.Id}:publish");
        data.ObserverByName(ScenarioObserver.Alice).PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{er}/incident-reports",
            new
            {
                id = Guid.NewGuid(),
                formId = form.Id,
                locationType = "NotRelatedToAPollingStation",
                locationDescription = "At the town hall"
            });

        var alfaForm = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormByCode("A");
        var betaForm = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Beta).FormByCode("B");
        return (data,
            alfaForm.GetSubmissionId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi),
            alfaForm.GetSubmissionId(ScenarioObserver.Alice, ScenarioPollingStation.Bacau),
            alfaForm.GetSubmissionId(ScenarioObserver.Alice, ScenarioPollingStation.Cluj),
            betaForm.GetSubmissionId(ScenarioObserver.Bob, ScenarioPollingStation.Iasi));
    }

    private static string MetricsUri(Guid electionRoundId, int? level = null) =>
        $"/api/election-rounds/{electionRoundId}/reviews:metrics" + (level is null ? "" : $"?level={level}");

    private static StageResponse[] Stages(HttpClient client, Guid electionRoundId) =>
        client.GetResponse<StageResponse[]>($"/api/election-rounds/{electionRoundId}/review-stages");

    private static Dictionary<string, int> ByStageName(StageResponse[] stages, StageCount[] counts)
    {
        counts.Select(x => x.StageId).Should().Equal(stages.Select(x => x.Id), "every stage is listed in workflow order");
        return counts.ToDictionary(x => stages.Single(s => s.Id == x.StageId).Name, x => x.Count);
    }

    private static int[] BySeverity(SeverityCount[] counts)
    {
        counts.Select(x => x.Severity).Should().Equal(SeverityOrder);
        return counts.Select(x => x.Count).ToArray();
    }

    private static void SetUp(HttpClient admin, Guid er, Guid iasi, Guid bacau)
    {
        var reviewed = Stages(admin, er).Single(x => x.Name == "Reviewed");
        admin.PutWithoutResponse($"/api/election-rounds/{er}/reviews/FormSubmission/{iasi}:stage",
            new { stageId = reviewed.Id });
        admin.PutWithoutResponse($"/api/election-rounds/{er}/severities/FormSubmission/{iasi}",
            new { severity = "Urgent" });
        admin.PutWithoutResponse($"/api/election-rounds/{er}/severities/FormSubmission/{bacau}",
            new { severity = "Low" });
    }

    [Test]
    public void CountsRecordsPerStageAndSeverityForEachRecordType()
    {
        var (data, iasi, bacau, _, _) = Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        SetUp(admin, er, iasi, bacau);
        var stages = Stages(admin, er);

        var metrics = admin.GetResponse<MetricsResponse>(MetricsUri(er));

        metrics.RecordTypes.Select(x => x.RecordType).Should()
            .Equal("FormSubmission", "QuickReport", "IncidentReport");

        var submissions = metrics.RecordTypes[0];
        submissions.Total.Should().Be(3);
        submissions.Places.Should().BeNull();
        ByStageName(stages, submissions.Stages).Should().BeEquivalentTo(new Dictionary<string, int>
        {
            ["Not reviewed"] = 2, ["In review"] = 0, ["Reviewed"] = 1
        });
        BySeverity(submissions.Severities).Should().Equal(1, 0, 0, 1, 1);

        var quickReports = metrics.RecordTypes[1];
        quickReports.Total.Should().Be(1);
        ByStageName(stages, quickReports.Stages)["Not reviewed"].Should().Be(1);
        BySeverity(quickReports.Severities).Should().Equal(0, 0, 0, 0, 1);

        var incidents = metrics.RecordTypes[2];
        incidents.Total.Should().Be(1);
        BySeverity(incidents.Severities).Should().Equal(0, 0, 0, 0, 1);
    }

    [Test]
    public void BreaksDownByPlaceAtLevelOneAndTwo()
    {
        var (data, iasi, bacau, _, _) = Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        SetUp(admin, er, iasi, bacau);
        var stages = Stages(admin, er);

        var level1 = admin.GetResponse<MetricsResponse>(MetricsUri(er, 1));
        var level2 = admin.GetResponse<MetricsResponse>(MetricsUri(er, 2));

        var submissionsByLevel1 = level1.RecordTypes.Single(x => x.RecordType == "FormSubmission").Places!;
        submissionsByLevel1.Select(x => (x.Level1, x.Level2, x.Total)).Should()
            .Equal(("North", null, 2), ("South", null, 1));
        ByStageName(stages, submissionsByLevel1[0].Stages)["Reviewed"].Should().Be(1);
        BySeverity(submissionsByLevel1[0].Severities).Should().Equal(1, 0, 0, 1, 0);
        BySeverity(submissionsByLevel1[1].Severities).Should().Equal(0, 0, 0, 0, 1);

        var incidentsByLevel1 = level1.RecordTypes.Single(x => x.RecordType == "IncidentReport").Places!;
        incidentsByLevel1.Should().ContainSingle();
        incidentsByLevel1[0].Should().Match<PlaceMetrics>(x =>
            x.Level1 == null && x.Level2 == null && x.Level5 == null && x.Total == 1);

        var submissionsByLevel2 = level2.RecordTypes.Single(x => x.RecordType == "FormSubmission").Places!;
        submissionsByLevel2.Select(x => (x.Level1, x.Level2, x.Level3, x.Total)).Should()
            .Equal(("North", "A", null, 1), ("North", "B", null, 1), (null, null, null, 1));
        BySeverity(submissionsByLevel2[0].Severities).Should().Equal(1, 0, 0, 0, 0);
        ByStageName(stages, submissionsByLevel2[1].Stages)["Not reviewed"].Should().Be(1);
    }

    [Test]
    public void CountsOnlyTheCallersNgoRecordsAndReview()
    {
        var (data, iasi, bacau, _, _) = Scenario();
        var er = data.ElectionRoundId;
        SetUp(data.NgoByName(ScenarioNgos.Alfa).Admin, er, iasi, bacau);
        var beta = data.NgoByName(ScenarioNgos.Beta).Admin;
        var stages = Stages(beta, er);

        var metrics = beta.GetResponse<MetricsResponse>(MetricsUri(er, 1));

        var submissions = metrics.RecordTypes.Single(x => x.RecordType == "FormSubmission");
        submissions.Total.Should().Be(1);
        ByStageName(stages, submissions.Stages)["Not reviewed"].Should().Be(1);
        BySeverity(submissions.Severities).Should().Equal(0, 0, 0, 0, 1);
        submissions.Places!.Select(x => (x.Level1, x.Total)).Should().Equal(("North", 1));
        metrics.RecordTypes.Single(x => x.RecordType == "QuickReport").Total.Should().Be(0);
        metrics.RecordTypes.Single(x => x.RecordType == "IncidentReport").Total.Should().Be(0);
    }

    [Test]
    public void CitizenReportsAreIncludedOnlyForTheCitizenReportingNgo()
    {
        var (data, _, _, _, _) = Scenario();
        var er = data.ElectionRoundId;
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);

        alfa.Admin.GetResponse<MetricsResponse>(MetricsUri(er)).RecordTypes
            .Should().NotContain(x => x.RecordType == "CitizenReport");

        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:enableCitizenReporting",
            new { ngoId = alfa.NgoId });

        var citizen = alfa.Admin.GetResponse<MetricsResponse>(MetricsUri(er, 1)).RecordTypes
            .Single(x => x.RecordType == "CitizenReport");
        citizen.Total.Should().Be(0);
        citizen.Stages.Should().HaveCount(3).And.OnlyContain(x => x.Count == 0);
        citizen.Severities.Select(x => x.Severity).Should().Equal(SeverityOrder);
        citizen.Places.Should().BeEmpty();
        beta.Admin.GetResponse<MetricsResponse>(MetricsUri(er)).RecordTypes
            .Should().NotContain(x => x.RecordType == "CitizenReport");
    }

    [Test]
    public void CoalitionLeadersCountTheMembersRecordsWithTheCoalitionDataSource()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
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
        var er = data.ElectionRoundId;
        var alfa = data.NgoByName(ScenarioNgos.Alfa).Admin;

        int Total(string dataSource) => alfa
            .GetResponse<MetricsResponse>($"/api/election-rounds/{er}/reviews:metrics?dataSource={dataSource}")
            .RecordTypes.Single(x => x.RecordType == "FormSubmission").Total;

        Total("Ngo").Should().Be(1);
        Total("Coalition").Should().Be(2);
    }

    [Test]
    public void StaffCanReadTheMetrics_PlatformAdminsCannot()
    {
        var (data, _, _, _, _) = Scenario();
        var er = data.ElectionRoundId;

        var staff = data.NgoByName(ScenarioNgos.Alfa).StaffByName(Sam).GetAsync(MetricsUri(er)).GetAwaiter()
            .GetResult();
        var platformAdmin = data.PlatformAdmin.GetAsync(MetricsUri(er)).GetAwaiter().GetResult();
        var badLevel = data.NgoByName(ScenarioNgos.Alfa).Admin.GetAsync(MetricsUri(er, 6)).GetAwaiter().GetResult();

        staff.StatusCode.Should().Be(HttpStatusCode.OK);
        staff.Content.ReadFromJsonAsync<MetricsResponse>().GetAwaiter().GetResult()!
            .RecordTypes.Single(x => x.RecordType == "FormSubmission").Total.Should().Be(3);
        platformAdmin.IsSuccessStatusCode.Should().BeFalse();
        badLevel.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
