extern alias HangfireJobs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NSubstitute;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Dapper;
using Vote.Monitor.Core.Converters;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.FormBase;
using Jobs = HangfireJobs::Vote.Monitor.Hangfire.Jobs.Export;

namespace Vote.Monitor.Api.IntegrationTests.Features.Labels;

using static ApiTesting;

public class LabelsTests : BaseApiTestFixture
{
    private const string Sam = "Sam";
    private const string Tom = "Tom";

    public record LabelsResponse(string[] Labels);

    public record HistoryEntryResponse(string Kind, string ActorName, string[]? AddedLabels, string[]? RemovedLabels);

    public record InboxItemResponse(Guid Id, string Reason, Guid RecordId, HistoryEntryResponse Activity);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    public record EntryResponse(Guid SubmissionId, string[] Labels);

    public record QuickReportEntryResponse(Guid Id, string[] Labels);

    public record IncidentReportEntryResponse(Guid IncidentReportId, string[] Labels);

    public record LabelCount(string Label, int Count);

    public record RecordTypeMetrics(string RecordType, int Total, LabelCount[] Labels, int Unlabelled);

    public record MetricsResponse(RecordTypeMetrics[] RecordTypes);

    public record BucketResponse(string? Key, string? Label, long Value);

    public record BarResponse(string? Key, string? Label, long Value, BucketResponse[]? Parts);

    public record InsightsResponse(string XAxis, string? GroupBy, long Total, BarResponse[] Bars, BucketResponse[]? Groups);

    public record ExportResponse(Guid ExportedDataId);

    /// <summary>The jsonb columns the export jobs read, registered by the Hangfire app at startup.</summary>
    [OneTimeSetUp]
    public void RegisterExportTypeHandlers()
    {
        SqlMapper.AddTypeHandler(typeof(BaseAnswer[]), new JsonToObjectConverter<BaseAnswer[]>());
        SqlMapper.AddTypeHandler(typeof(SubmissionAttachmentModel[]), new JsonToObjectConverter<SubmissionAttachmentModel[]>());
        SqlMapper.AddTypeHandler(typeof(Jobs.QuickReports.ReadModels.QuickReportAttachmentModel[]),
            new JsonToObjectConverter<Jobs.QuickReports.ReadModels.QuickReportAttachmentModel[]>());
        SqlMapper.AddTypeHandler(typeof(Jobs.ReadModels.CommentModel[]),
            new JsonToObjectConverter<Jobs.ReadModels.CommentModel[]>());
    }

    /// <summary>Alfa leads a coalition with Beta; Alice (Alfa) and Bob (Beta) each submitted the common form.</summary>
    private (ScenarioData Data, Guid AliceSubmissionId, Guid BobSubmissionId) CoalitionScenario()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithStaff(Sam).WithStaff(Tom))
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

    /// <summary>Alfa's Alice sent two quick reports (Iasi, Cluj) and one incident report.</summary>
    private (ScenarioData Data, Guid IasiQuickReport, Guid ClujQuickReport, Guid IncidentReport) ReportsScenario()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Cluj))
            .Please();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        var incidentForm = Dummy.Form("INC");
        incidentForm.FormType = FormType.IncidentReporting;
        var form = admin.PostWithResponse<CreateFormRequest>($"/api/election-rounds/{er}/forms", incidentForm);
        admin.PostWithoutResponse($"/api/election-rounds/{er}/forms/{form.Id}:publish");
        var incidentReportId = Guid.NewGuid();
        data.ObserverByName(ScenarioObserver.Alice).PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{er}/incident-reports",
            new
            {
                id = incidentReportId,
                formId = form.Id,
                locationType = "NotRelatedToAPollingStation",
                locationDescription = "At the town hall"
            });

        return (data,
            data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi),
            data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Cluj),
            incidentReportId);
    }

    private static string LabelsUri(Guid electionRoundId, Guid recordId, string recordType = "FormSubmission") =>
        $"/api/election-rounds/{electionRoundId}/labels/{recordType}/{recordId}";

    private static string ReviewUri(Guid electionRoundId, Guid recordId, string recordType = "FormSubmission") =>
        $"/api/election-rounds/{electionRoundId}/reviews/{recordType}/{recordId}";

    private static HttpResponseMessage Put(HttpClient client, string uri, params string[] labels) =>
        client.PutAsJsonAsync(uri, new { labels }).GetAwaiter().GetResult();

    private static string[] LabelsOf(HttpClient client, string uri) => client.GetResponse<LabelsResponse>(uri).Labels;

    private static List<EntryResponse> Entries(HttpClient client, Guid electionRoundId, object? filter = null) =>
        client.PostWithResponse<PagedResponse<EntryResponse>>(
            $"/api/election-rounds/{electionRoundId}/form-submissions:byEntryV2",
            new { dataSource = "Coalition", filter, pageSize = 100 }).Items;

    private static List<EntryResponse> EntriesV1(HttpClient client, Guid electionRoundId, string query) =>
        client.GetResponse<PagedResponse<EntryResponse>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource=Coalition&pageSize=100&{query}")
            .Items;

    private static string Body(HttpResponseMessage response) =>
        response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

    [Test]
    public void RecordsStartWithoutLabels()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        LabelsOf(admin, LabelsUri(data.ElectionRoundId, aliceSubmissionId)).Should().BeEmpty();
        LabelsOf(admin, $"/api/election-rounds/{data.ElectionRoundId}/labels").Should().BeEmpty();
        Entries(admin, data.ElectionRoundId).Should().HaveCount(2).And.OnlyContain(x => x.Labels.Length == 0);
        EntriesV1(admin, data.ElectionRoundId, "").Should().HaveCount(2).And.OnlyContain(x => x.Labels.Length == 0);
    }

    [Test]
    public void SettingLabelsReplacesThem_TrimmedAndOnceIgnoringCase()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        var uri = LabelsUri(er, aliceSubmissionId);

        Put(admin, uri, "  Media ", "media", "Escalated", "", "   ").StatusCode.Should().Be(HttpStatusCode.NoContent);
        LabelsOf(admin, uri).Should().Equal("Escalated", "Media");

        Put(admin, uri, "Media", "Ballot secrecy").StatusCode.Should().Be(HttpStatusCode.NoContent);
        LabelsOf(admin, uri).Should().Equal("Ballot secrecy", "Media");

        // A label the NGO already uses in the round keeps that spelling.
        Put(admin, LabelsUri(er, bobSubmissionId), "MEDIA", "Needs legal review");
        LabelsOf(admin, LabelsUri(er, bobSubmissionId)).Should().Equal("Media", "Needs legal review");

        LabelsOf(admin, $"/api/election-rounds/{er}/labels").Should()
            .Equal("Ballot secrecy", "Media", "Needs legal review");

        Put(admin, uri).StatusCode.Should().Be(HttpStatusCode.NoContent);
        LabelsOf(admin, uri).Should().BeEmpty();
    }

    [Test]
    public void LabelsLongerThan64CharactersAreRejected()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var uri = LabelsUri(data.ElectionRoundId, aliceSubmissionId);

        Put(admin, uri, new string('x', 65)).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Put(admin, uri, "  " + new string('x', 64) + "  ").StatusCode.Should().Be(HttpStatusCode.NoContent);
        admin.PutAsJsonAsync(uri, new { labels = (string[]?)null }).GetAwaiter().GetResult().StatusCode
            .Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public void ChangesAreRecordedOnceInTheHistory_AndReviewersAreNotified()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var uri = LabelsUri(er, aliceSubmissionId);
        alfa.Admin.PutWithoutResponse($"{ReviewUri(er, aliceSubmissionId)}:reviewers",
            new { userIds = new[] { alfa.StaffIdByName(Sam), alfa.AdminId } });

        Put(alfa.Admin, uri, "Media", "Escalated");
        Put(alfa.Admin, uri, "escalated", "MEDIA"); // the same labels: no change
        Put(alfa.Admin, uri, "Media", "Ballot secrecy");

        var history = alfa.Admin.GetResponse<HistoryEntryResponse[]>($"{ReviewUri(er, aliceSubmissionId)}/history");
        var changes = history.Where(x => x.Kind == "LabelsChanged").ToList();
        changes.Should().HaveCount(2);
        changes.Should().ContainEquivalentOf(new
        {
            AddedLabels = new[] { "Escalated", "Media" }, RemovedLabels = Array.Empty<string>()
        });
        changes.Should().ContainEquivalentOf(new
        {
            AddedLabels = new[] { "Ballot secrecy" }, RemovedLabels = new[] { "Escalated" }
        });
        history.Where(x => x.Kind != "LabelsChanged")
            .Should().OnlyContain(x => x.AddedLabels == null && x.RemovedLabels == null);

        var raw = JsonDocument.Parse(alfa.Admin.GetStringAsync($"{ReviewUri(er, aliceSubmissionId)}/history")
            .GetAwaiter().GetResult()).RootElement;
        raw.EnumerateArray().Select(x => x.TryGetProperty("addedLabels", out var added)
                                         && x.TryGetProperty("removedLabels", out var removed))
            .Should().OnlyContain(present => present, "every entry carries addedLabels and removedLabels");

        var samInbox = alfa.StaffByName(Sam).GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox");
        samInbox.Items.Where(x => x.Activity.Kind == "LabelsChanged").Should().HaveCount(2)
            .And.OnlyContain(x => x.Reason == "ReviewingRecordChanged" && x.RecordId == aliceSubmissionId);
        samInbox.Items.Should().ContainSingle(x => x.Activity.Kind == "LabelsChanged"
                                                   && x.Activity.RemovedLabels!.SequenceEqual(new[] { "Escalated" }));
        alfa.Admin.GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox").Items
            .Should().NotContain(x => x.Activity.Kind == "LabelsChanged");
    }

    [Test]
    public void StaffLabelAnyRecord_EvenInAnArchivedRound()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;

        Put(alfa.StaffByName(Tom), LabelsUri(er, aliceSubmissionId), "Media").StatusCode
            .Should().Be(HttpStatusCode.NoContent);
        LabelsOf(alfa.StaffByName(Sam), LabelsUri(er, aliceSubmissionId)).Should().Equal("Media");

        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:archive", new { });

        Put(alfa.StaffByName(Tom), LabelsUri(er, aliceSubmissionId), "Media", "Escalated").StatusCode
            .Should().Be(HttpStatusCode.NoContent);
        LabelsOf(alfa.Admin, LabelsUri(er, aliceSubmissionId)).Should().Equal("Escalated", "Media");
    }

    [Test]
    public void PlatformAdminsCannotReadOrSetLabels()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var er = data.ElectionRoundId;

        Put(data.PlatformAdmin, LabelsUri(er, aliceSubmissionId), "Media").StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        data.PlatformAdmin.GetAsync(LabelsUri(er, aliceSubmissionId)).GetAwaiter().GetResult().StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        data.PlatformAdmin.GetAsync($"/api/election-rounds/{er}/labels").GetAwaiter().GetResult().StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public void EachNgoKeepsItsOwnLabels()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;

        Put(alfa.Admin, LabelsUri(er, bobSubmissionId), "Media");

        LabelsOf(beta.Admin, LabelsUri(er, bobSubmissionId)).Should().BeEmpty();
        LabelsOf(beta.Admin, $"/api/election-rounds/{er}/labels").Should().BeEmpty();
        beta.Admin.PostWithResponse<PagedResponse<EntryResponse>>(
                $"/api/election-rounds/{er}/form-submissions:byEntryV2", new { dataSource = "Ngo" })
            .Items.Single(x => x.SubmissionId == bobSubmissionId).Labels.Should().BeEmpty();
        beta.Admin.GetResponse<HistoryEntryResponse[]>($"{ReviewUri(er, bobSubmissionId)}/history")
            .Should().NotContain(x => x.Kind == "LabelsChanged");

        Put(beta.Admin, LabelsUri(er, bobSubmissionId), "Beta only");
        LabelsOf(alfa.Admin, LabelsUri(er, bobSubmissionId)).Should().Equal("Media");

        // Beta can't see Alice's submission (the coalition's leader's observer), so can't label it.
        Put(beta.Admin, LabelsUri(er, aliceSubmissionId), "Media").StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public void SubmissionListsCarryAndFilterByLabels_IgnoringCase()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        Put(admin, LabelsUri(er, aliceSubmissionId), "Media", "Escalated");

        var all = Entries(admin, er);
        all.Single(x => x.SubmissionId == aliceSubmissionId).Labels.Should().Equal("Escalated", "Media");
        all.Single(x => x.SubmissionId == bobSubmissionId).Labels.Should().BeEmpty();

        Entries(admin, er, new { op = "in", field = "labels", values = new[] { "MEDIA", "Other" } })
            .Select(x => x.SubmissionId).Should().Equal(aliceSubmissionId);
        Entries(admin, er, new { op = "notIn", field = "labels", values = new[] { "media" } })
            .Select(x => x.SubmissionId).Should().Equal(bobSubmissionId);
        Entries(admin, er, new { op = "isEmpty", field = "labels" })
            .Select(x => x.SubmissionId).Should().Equal(bobSubmissionId);
        Entries(admin, er, new { op = "isNotEmpty", field = "labels" })
            .Select(x => x.SubmissionId).Should().Equal(aliceSubmissionId);
        admin.PostAsJsonAsync($"/api/election-rounds/{er}/form-submissions:byEntryV2",
                new { dataSource = "Coalition", filter = new { op = "eq", field = "labels", value = "Media" } })
            .GetAwaiter().GetResult().IsSuccessStatusCode.Should().BeFalse("labels take in / notIn / isEmpty / isNotEmpty");

        EntriesV1(admin, er, "labels=escalated").Select(x => x.SubmissionId).Should().Equal(aliceSubmissionId);
        EntriesV1(admin, er, "labels=Other&labels=Media").Select(x => x.SubmissionId).Should().Equal(aliceSubmissionId);
        EntriesV1(admin, er, "labels=Other").Should().BeEmpty();
        EntriesV1(admin, er, "").Single(x => x.SubmissionId == aliceSubmissionId).Labels
            .Should().Equal("Escalated", "Media");

        // The aggregated answers take the same filter (run inside a PL/pgSQL function).
        var formId = data.ElectionRound.Coalition.FormId;
        using var aggregate = JsonDocument.Parse(admin.PostWithResponse<JsonElement>(
            $"/api/election-rounds/{er}/form-submissions/{formId}:aggregatedV2",
            new { dataSource = "Coalition", filter = new { op = "in", field = "labels", values = new[] { "media" } } })
            .GetRawText());
        aggregate.RootElement.GetProperty("submissionsAggregate").GetProperty("submissionCount").GetInt32()
            .Should().Be(1);
    }

    [Test]
    public void QuickAndIncidentReportListsCarryAndFilterByLabels()
    {
        var (data, iasi, cluj, incidentReportId) = ReportsScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        Put(admin, LabelsUri(er, iasi, "QuickReport"), "Media").StatusCode.Should().Be(HttpStatusCode.NoContent);
        Put(admin, LabelsUri(er, incidentReportId, "IncidentReport"), "Escalated").StatusCode
            .Should().Be(HttpStatusCode.NoContent);

        var v1 = admin.GetResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports?dataSource=Ngo");
        v1.Items.Single(x => x.Id == iasi).Labels.Should().Equal("Media");
        v1.Items.Single(x => x.Id == cluj).Labels.Should().BeEmpty();
        var v1Media = admin.GetResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports?dataSource=Ngo&labels=media");
        v1Media.Items.Select(x => x.Id).Should().Equal(iasi);
        v1Media.TotalCount.Should().Be(1);

        var v2 = admin.PostWithResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports:listV2",
            new { dataSource = "Ngo", filter = new { op = "isEmpty", field = "labels" } });
        v2.Items.Select(x => x.Id).Should().Equal(cluj);
        admin.PostWithResponse<PagedResponse<QuickReportEntryResponse>>(
                $"/api/election-rounds/{er}/quick-reports:listV2",
                new { dataSource = "Ngo", filter = new { op = "in", field = "labels", values = new[] { "Media" } } })
            .Items.Single().Labels.Should().Equal("Media");

        var ir = admin.GetResponse<PagedResponse<IncidentReportEntryResponse>>(
            $"/api/election-rounds/{er}/incident-reports:byEntry");
        ir.Items.Single().Labels.Should().Equal("Escalated");
        admin.GetResponse<PagedResponse<IncidentReportEntryResponse>>(
                $"/api/election-rounds/{er}/incident-reports:byEntry?labels=ESCALATED")
            .TotalCount.Should().Be(1);
        var irNone = admin.GetResponse<PagedResponse<IncidentReportEntryResponse>>(
            $"/api/election-rounds/{er}/incident-reports:byEntry?labels=Media");
        irNone.Items.Should().BeEmpty();
        irNone.TotalCount.Should().Be(0);

        var irV2 = admin.PostWithResponse<PagedResponse<IncidentReportEntryResponse>>(
            $"/api/election-rounds/{er}/incident-reports:byEntryV2",
            new { dataSource = "Ngo", filter = new { op = "notIn", field = "labels", values = new[] { "Escalated" } } });
        irV2.Items.Should().BeEmpty();
        admin.PostWithResponse<PagedResponse<IncidentReportEntryResponse>>(
                $"/api/election-rounds/{er}/incident-reports:byEntryV2", new { dataSource = "Ngo" })
            .Items.Single().Labels.Should().Equal("Escalated");
    }

    [Test]
    public void CitizenReportListsAcceptTheLabelsFilter()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:enableCitizenReporting",
            new { ngoId = alfa.NgoId });

        var responses = new[]
        {
            alfa.Admin.GetAsync($"/api/election-rounds/{er}/citizen-reports:byEntry?labels=Media").GetAwaiter().GetResult(),
            alfa.Admin.PostAsJsonAsync($"/api/election-rounds/{er}/citizen-reports:byEntryV2",
                new { filter = new { op = "notIn", field = "labels", values = new[] { "Media" } } }).GetAwaiter().GetResult(),
            alfa.Admin.PostAsJsonAsync($"/api/election-rounds/{er}/citizen-reports:byEntryV2",
                new { filter = new { op = "isNotEmpty", field = "labels" } }).GetAwaiter().GetResult()
        };

        foreach (var response in responses)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK, Body(response));
        }
    }

    [Test]
    public void DeletingARecordRemovesItsLabels()
    {
        var (data, iasi, _, _) = ReportsScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        Put(admin, LabelsUri(er, iasi, "QuickReport"), "Media");

        data.ObserverByName(ScenarioObserver.Alice)
            .DeleteAsync($"/api/election-rounds/{er}/quick-reports/{iasi}").GetAwaiter().GetResult()
            .EnsureSuccessStatusCode();

        LabelsOf(admin, $"/api/election-rounds/{er}/labels").Should().BeEmpty();
    }

    [Test]
    public void MetricsCountTheRecordsPerLabel()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        Put(admin, LabelsUri(er, aliceSubmissionId), "Media", "Escalated");
        Put(admin, LabelsUri(er, bobSubmissionId), "Media");

        var coalition = admin.GetResponse<MetricsResponse>($"/api/election-rounds/{er}/reviews:metrics?dataSource=Coalition");
        var submissions = coalition.RecordTypes.Single(x => x.RecordType == "FormSubmission");
        submissions.Total.Should().Be(2);
        submissions.Labels.Should().Equal(new LabelCount("Media", 2), new LabelCount("Escalated", 1));
        submissions.Unlabelled.Should().Be(0);
        coalition.RecordTypes.Single(x => x.RecordType == "QuickReport").Labels.Should().BeEmpty();

        // The NGO's own data only: Bob is Beta's observer.
        var own = admin.GetResponse<MetricsResponse>($"/api/election-rounds/{er}/reviews:metrics")
            .RecordTypes.Single(x => x.RecordType == "FormSubmission");
        own.Labels.Should().Equal(new LabelCount("Escalated", 1), new LabelCount("Media", 1));

        Put(admin, LabelsUri(er, bobSubmissionId));
        admin.GetResponse<MetricsResponse>($"/api/election-rounds/{er}/reviews:metrics?dataSource=Coalition")
            .RecordTypes.Single(x => x.RecordType == "FormSubmission").Unlabelled.Should().Be(1);
    }

    [Test]
    public void InsightsSplitByLabel_CountingARecordOncePerLabelButOnceInEveryTotal()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        Put(admin, LabelsUri(er, aliceSubmissionId), "Media", "Escalated");
        Put(admin, LabelsUri(er, bobSubmissionId), "Media");
        var uri = $"/api/election-rounds/{er}/reviews/FormSubmission:insights";

        var byLabel = admin.PostWithResponse<InsightsResponse>(uri, new { dataSource = "Coalition", xAxis = "label" });
        byLabel.Total.Should().Be(2);
        byLabel.Bars.Select(x => (x.Key, x.Label, x.Value)).Should()
            .Equal(("Media", "Media", 2L), ("Escalated", "Escalated", 1L));

        var grouped = admin.PostWithResponse<InsightsResponse>(uri,
            new { dataSource = "Coalition", xAxis = "severity", groupBy = "label" });
        grouped.Total.Should().Be(2);
        grouped.Bars.Should().ContainSingle().Which.Value.Should().Be(2);
        grouped.Bars[0].Parts!.Select(x => (x.Key, x.Value)).Should().Equal(("Media", 2L), ("Escalated", 1L));
        grouped.Groups!.Select(x => (x.Key, x.Value)).Should().Equal(("Media", 2L), ("Escalated", 1L));

        Put(admin, LabelsUri(er, bobSubmissionId));
        var withUnlabelled = admin.PostWithResponse<InsightsResponse>(uri,
            new { dataSource = "Coalition", xAxis = "label", filter = new { op = "isNotEmpty", field = "labels" } });
        withUnlabelled.Total.Should().Be(1);
        admin.PostWithResponse<InsightsResponse>(uri, new { dataSource = "Coalition", xAxis = "label" })
            .Bars.Select(x => (x.Key, x.Value)).Should().Equal(("Escalated", 1L), ("Media", 1L), (null, 1L));
    }

    [Test]
    public void ExportsHaveALabelsColumnAfterTheFollowUpStatus_AndHonourTheLabelsFilter()
    {
        var (data, iasi, cluj, incidentReportId) = ReportsScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        Put(admin, LabelsUri(er, iasi, "QuickReport"), "Media", "Ballot secrecy");
        Put(admin, LabelsUri(er, incidentReportId, "IncidentReport"), "Escalated");

        var quickReports = Export(admin, er, "QuickReports", null,
            id => RunJob(id, (ctx, db, files, time) => new Jobs.QuickReports.ExportQuickReportsJob(ctx, db, files,
                NullLogger<Jobs.FormSubmissions.ExportFormSubmissionsJob>.Instance, time).Run(er, data.NgoByName(ScenarioNgos.Alfa).NgoId, id, default)));
        LabelsColumn(quickReports, "quick-reports").Should().BeEquivalentTo(
            new Dictionary<string, string> { [iasi.ToString()] = "Ballot secrecy; Media", [cluj.ToString()] = "" });

        var filtered = Export(admin, er, "QuickReports", new { op = "in", field = "labels", values = new[] { "media" } },
            id => RunJob(id, (ctx, db, files, time) => new Jobs.QuickReports.ExportQuickReportsJob(ctx, db, files,
                NullLogger<Jobs.FormSubmissions.ExportFormSubmissionsJob>.Instance, time).Run(er, data.NgoByName(ScenarioNgos.Alfa).NgoId, id, default)));
        LabelsColumn(filtered, "quick-reports").Keys.Should().Equal(iasi.ToString());

        var incidentReports = Export(admin, er, "IncidentReports", null,
            id => RunJob(id, (ctx, db, files, time) => new Jobs.IncidentReports.ExportIncidentReportsJob(ctx, db, files,
                NullLogger<Jobs.IncidentReports.ExportIncidentReportsJob>.Instance, time).Run(er, data.NgoByName(ScenarioNgos.Alfa).NgoId, id, default)));
        LabelsColumn(incidentReports, null).Should().BeEquivalentTo(
            new Dictionary<string, string> { [incidentReportId.ToString()] = "Escalated" });
    }

    [Test]
    public void SubmissionsExportHasALabelsColumnAfterTheFollowUpStatus()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        Put(alfa.Admin, LabelsUri(er, aliceSubmissionId), "Media", "Escalated");

        foreach (var simplified in new[] { false, true })
        {
            var workbook = ExportV2(alfa.Admin, er, simplified ? "FormSubmissionsSimplified" : "FormSubmissions",
                new { op = "isNotEmpty", field = "labels" }, "Coalition",
                id => RunJob(id, (ctx, db, files, time) => simplified
                    ? new Jobs.FormSubmissions.ExportFormSubmissionsSimplifiedJob(ctx, db, files,
                        NullLogger<Jobs.FormSubmissions.ExportFormSubmissionsSimplifiedJob>.Instance, time).Run(er, alfa.NgoId, id, default)
                    : new Jobs.FormSubmissions.ExportFormSubmissionsJob(ctx, db, files,
                        NullLogger<Jobs.FormSubmissions.ExportFormSubmissionsJob>.Instance, time).Run(er, alfa.NgoId, id, default)));

            var rows = LabelsColumn(workbook, null);
            rows.Should().BeEquivalentTo(new Dictionary<string, string> { [aliceSubmissionId.ToString()] = "Escalated; Media" },
                simplified ? "the simplified export" : "the export");
            rows.Should().NotContainKey(bobSubmissionId.ToString());
        }
    }

    private static IWorkbook Export(HttpClient client, Guid electionRoundId, string type, object? filter,
        Func<Guid, Task> run) => ExportV2(client, electionRoundId, type, filter, "Ngo", run);

    private static IWorkbook ExportV2(HttpClient client, Guid electionRoundId, string type, object? filter,
        string dataSource, Func<Guid, Task> run)
    {
        var export = client.PostWithResponse<ExportResponse>("/api/exported-data:v2",
            new { exportedDataType = type, electionRoundId, dataSource, filter });
        run(export.ExportedDataId).GetAwaiter().GetResult();

        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VoteMonitorContext>();
        var exported = context.ExportedData.AsNoTracking().Single(x => x.Id == export.ExportedDataId);
        exported.Base64EncodedData.Should().NotBeNull();
        return new XSSFWorkbook(new MemoryStream(Convert.FromBase64String(exported.Base64EncodedData!)));
    }

    /// <summary>Runs an export job with the API's services (Hangfire is off in the tests).</summary>
    private static async Task RunJob(Guid exportedDataId,
        Func<VoteMonitorContext, INpgsqlConnectionFactory, IFileStorageService, ITimeProvider, Task> run)
    {
        using var scope = Services.CreateScope();
        var files = Substitute.For<IFileStorageService>();
        await run(scope.ServiceProvider.GetRequiredService<VoteMonitorContext>(),
            scope.ServiceProvider.GetRequiredService<INpgsqlConnectionFactory>(),
            files,
            ApiTimeProvider);
    }

    /// <summary>
    /// Each data row's id (first column) and its Labels cell, over the given sheet or every sheet with a Labels
    /// column. The Labels column comes right after FollowUpStatus.
    /// </summary>
    private static Dictionary<string, string> LabelsColumn(IWorkbook workbook, string? sheetName)
    {
        var result = new Dictionary<string, string>();
        for (var i = 0; i < workbook.NumberOfSheets; i++)
        {
            var sheet = workbook.GetSheetAt(i);
            if (sheetName is not null && sheet.SheetName != sheetName)
            {
                continue;
            }

            var header = sheet.GetRow(0).Cells.Select(c => c.StringCellValue).ToList();
            var column = header.IndexOf("Labels");
            if (column < 0)
            {
                continue;
            }

            column.Should().Be(header.IndexOf("FollowUpStatus") + 1, $"Labels follows FollowUpStatus in {sheet.SheetName}");
            for (var r = 1; r <= sheet.LastRowNum; r++)
            {
                var row = sheet.GetRow(r);
                if (row is null)
                {
                    continue;
                }

                result[row.GetCell(0).ToString()!] = row.GetCell(column)?.ToString() ?? "";
            }
        }

        return result;
    }
}
