using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Severity;

using static ApiTesting;

public class SeverityTests : BaseApiTestFixture
{
    private const string Sam = "Sam";
    private const string Tom = "Tom";

    public record SeverityResponse(string Severity, DateTime? ChangedAt);

    public record HistoryEntryResponse(string Kind, string ActorName, string? FromSeverity, string? ToSeverity);

    public record InboxItemResponse(Guid Id, string Reason, Guid RecordId, HistoryEntryResponse Activity);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    public record EntryResponse(Guid SubmissionId, string Severity);

    public record QuickReportEntryResponse(Guid Id, string Severity);

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

    private static string SeverityUri(Guid electionRoundId, Guid recordId, string recordType = "FormSubmission") =>
        $"/api/election-rounds/{electionRoundId}/severities/{recordType}/{recordId}";

    private static string ReviewUri(Guid electionRoundId, Guid recordId, string recordType = "FormSubmission") =>
        $"/api/election-rounds/{electionRoundId}/reviews/{recordType}/{recordId}";

    private static HttpResponseMessage Put(HttpClient client, string uri, object body) =>
        client.PutAsJsonAsync(uri, body).GetAwaiter().GetResult();

    private static List<EntryResponse> Entries(HttpClient client, Guid electionRoundId, string dataSource,
        object? filter = null, string? sortColumnName = null, string? sortOrder = null) =>
        client.PostWithResponse<PagedResponse<EntryResponse>>(
            $"/api/election-rounds/{electionRoundId}/form-submissions:byEntryV2",
            new { dataSource, filter, pageSize = 100, sortColumnName, sortOrder }).Items;

    private static List<EntryResponse> EntriesV1(HttpClient client, Guid electionRoundId, string query) =>
        client.GetResponse<PagedResponse<EntryResponse>>(
            $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource=Coalition&pageSize=100&{query}")
            .Items;

    private static string Body(HttpResponseMessage response) =>
        response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

    [Test]
    public void RecordsStartWithoutSeverity()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        var response = admin.GetAsync(SeverityUri(data.ElectionRoundId, aliceSubmissionId)).GetAwaiter().GetResult();
        var json = JsonDocument.Parse(Body(response)).RootElement;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json.GetProperty("severity").GetString().Should().Be("None");
        json.GetProperty("changedAt").ValueKind.Should().Be(JsonValueKind.Null);
        Entries(admin, data.ElectionRoundId, "Coalition").Should().HaveCount(2)
            .And.OnlyContain(x => x.Severity == "None");
        EntriesV1(admin, data.ElectionRoundId, "").Should().HaveCount(2).And.OnlyContain(x => x.Severity == "None");
    }

    [Test]
    public void AdminSetsSeverity_ListsShowFilterAndSortByIt()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;

        Put(admin, SeverityUri(er, aliceSubmissionId), new { severity = "Urgent" }).StatusCode
            .Should().Be(HttpStatusCode.NoContent);
        Put(admin, SeverityUri(er, bobSubmissionId), new { severity = "Low" }).StatusCode
            .Should().Be(HttpStatusCode.NoContent);

        var severity = admin.GetResponse<SeverityResponse>(SeverityUri(er, aliceSubmissionId));
        severity.Severity.Should().Be("Urgent");
        severity.ChangedAt.Should().NotBeNull();

        var all = Entries(admin, er, "Coalition");
        all.Single(x => x.SubmissionId == aliceSubmissionId).Severity.Should().Be("Urgent");
        all.Single(x => x.SubmissionId == bobSubmissionId).Severity.Should().Be("Low");

        Entries(admin, er, "Coalition", new { op = "eq", field = "severity", value = "Urgent" })
            .Select(x => x.SubmissionId).Should().Equal(aliceSubmissionId);
        Entries(admin, er, "Coalition", new { op = "in", field = "severity", values = new[] { "Low", "High" } })
            .Select(x => x.SubmissionId).Should().Equal(bobSubmissionId);
        Entries(admin, er, "Coalition", sortColumnName: "severity", sortOrder: "Desc")
            .Select(x => x.SubmissionId).Should().Equal(aliceSubmissionId, bobSubmissionId);
        Entries(admin, er, "Coalition", sortColumnName: "severity", sortOrder: "Asc")
            .Select(x => x.SubmissionId).Should().Equal(bobSubmissionId, aliceSubmissionId);

        EntriesV1(admin, er, "severity=Urgent").Select(x => x.SubmissionId).Should().Equal(aliceSubmissionId);
        EntriesV1(admin, er, "severity=High&severity=Urgent").Select(x => x.SubmissionId)
            .Should().Equal(aliceSubmissionId);
        EntriesV1(admin, er, "severity=Low&severity=Urgent").Should().HaveCount(2);
        EntriesV1(admin, er, "severity=None").Should().BeEmpty();
        EntriesV1(admin, er, "sortColumnName=severity&sortOrder=Desc").Select(x => x.SubmissionId)
            .Should().Equal(aliceSubmissionId, bobSubmissionId);
        EntriesV1(admin, er, "sortColumnName=severity&sortOrder=Asc").Select(x => x.SubmissionId)
            .Should().Equal(bobSubmissionId, aliceSubmissionId);
    }

    [Test]
    public void QuickReportListsCarryFilterAndSortBySeverity()
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
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        var iasi = data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var cluj = data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Cluj);

        admin.PutWithoutResponse(SeverityUri(er, iasi, "QuickReport"), new { severity = "High" });

        var v1 = admin.GetResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports?dataSource=Ngo");
        var v1High = admin.GetResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports?dataSource=Ngo&severity=High");
        var v1Sorted = admin.GetResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports?dataSource=Ngo&sortColumnName=severity&sortOrder=Asc");
        var v2None = admin.PostWithResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports:listV2",
            new { dataSource = "Ngo", filter = new { op = "eq", field = "severity", value = "None" } });
        var v2Sorted = admin.PostWithResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports:listV2",
            new { dataSource = "Ngo", sortColumnName = "severity", sortOrder = "Desc" });

        v1.Items.Single(x => x.Id == iasi).Severity.Should().Be("High");
        v1.Items.Single(x => x.Id == cluj).Severity.Should().Be("None");
        v1High.Items.Select(x => x.Id).Should().Equal(iasi);
        v1Sorted.Items.Select(x => x.Id).Should().Equal(cluj, iasi);
        v2None.Items.Select(x => x.Id).Should().Equal(cluj);
        v2Sorted.Items.Select(x => x.Id).Should().Equal(iasi, cluj);
    }

    [Test]
    public void IncidentAndCitizenListsAcceptTheSeverityFilterAndSort()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:enableCitizenReporting",
            new { ngoId = alfa.NgoId });
        const string query = "severity=High&severity=Urgent&sortColumnName=severity&sortOrder=Desc";
        var v2Body = new
        {
            dataSource = "Coalition",
            sortColumnName = "severity",
            sortOrder = "Desc",
            filter = new { op = "in", field = "severity", values = new[] { "High", "Urgent" } }
        };

        var responses = new[]
        {
            alfa.Admin.GetAsync($"/api/election-rounds/{er}/incident-reports:byEntry?{query}").GetAwaiter().GetResult(),
            alfa.Admin.GetAsync($"/api/election-rounds/{er}/citizen-reports:byEntry?{query}").GetAwaiter().GetResult(),
            alfa.Admin.PostAsJsonAsync($"/api/election-rounds/{er}/incident-reports:byEntryV2", v2Body)
                .GetAwaiter().GetResult(),
            alfa.Admin.PostAsJsonAsync($"/api/election-rounds/{er}/citizen-reports:byEntryV2", v2Body)
                .GetAwaiter().GetResult()
        };

        foreach (var response in responses)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK, Body(response));
        }
    }

    [Test]
    public void StaffChangeSeverityOnAnyRecord()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        alfa.Admin.PutWithoutResponse($"{ReviewUri(er, aliceSubmissionId)}:reviewers",
            new { userIds = new[] { alfa.StaffIdByName(Sam) } });

        // Tom doesn't review the report.
        Put(alfa.StaffByName(Tom), SeverityUri(er, aliceSubmissionId), new { severity = "High" }).StatusCode
            .Should().Be(HttpStatusCode.NoContent);
        Put(alfa.StaffByName(Sam), SeverityUri(er, aliceSubmissionId), new { severity = "Urgent" }).StatusCode
            .Should().Be(HttpStatusCode.NoContent);

        alfa.StaffByName(Tom).GetResponse<SeverityResponse>(SeverityUri(er, aliceSubmissionId)).Severity
            .Should().Be("Urgent");
    }

    [Test]
    public void ChangesAreRecordedOnceInTheHistory_AndReviewersAreNotified()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var uri = SeverityUri(er, aliceSubmissionId);
        alfa.Admin.PutWithoutResponse($"{ReviewUri(er, aliceSubmissionId)}:reviewers",
            new { userIds = new[] { alfa.StaffIdByName(Sam), alfa.AdminId } });

        alfa.Admin.PutWithoutResponse(uri, new { severity = "Urgent" });
        alfa.Admin.PutWithoutResponse(uri, new { severity = "Urgent" });

        var history = alfa.Admin.GetResponse<HistoryEntryResponse[]>($"{ReviewUri(er, aliceSubmissionId)}/history");
        history.Where(x => x.Kind == "SeverityChanged").Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new { FromSeverity = "None", ToSeverity = "Urgent" });
        history.Where(x => x.Kind != "SeverityChanged").Should().OnlyContain(x => x.FromSeverity == null && x.ToSeverity == null);

        var samInbox = alfa.StaffByName(Sam).GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox");
        samInbox.Items.Should().ContainSingle(x => x.Activity.Kind == "SeverityChanged")
            .Which.Should().BeEquivalentTo(new
            {
                Reason = "ReviewingRecordChanged",
                RecordId = aliceSubmissionId,
                Activity = new { FromSeverity = "None", ToSeverity = "Urgent" }
            });
        alfa.Admin.GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox").Items
            .Should().NotContain(x => x.Activity.Kind == "SeverityChanged");
    }

    [Test]
    public void EachNgoKeepsItsOwnSeverity()
    {
        var (data, _, bobSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;

        alfa.Admin.PutWithoutResponse(SeverityUri(er, bobSubmissionId), new { severity = "Urgent" });

        beta.Admin.GetResponse<SeverityResponse>(SeverityUri(er, bobSubmissionId)).Severity.Should().Be("None");
        Entries(beta.Admin, er, "Ngo").Single(x => x.SubmissionId == bobSubmissionId).Severity.Should().Be("None");
        Entries(beta.Admin, er, "Ngo", new { op = "eq", field = "severity", value = "Urgent" }).Should().BeEmpty();
        beta.Admin.GetResponse<HistoryEntryResponse[]>($"{ReviewUri(er, bobSubmissionId)}/history")
            .Should().NotContain(x => x.Kind == "SeverityChanged");
        Entries(alfa.Admin, er, "Coalition").Single(x => x.SubmissionId == bobSubmissionId).Severity
            .Should().Be("Urgent");

        beta.Admin.PutWithoutResponse(SeverityUri(er, bobSubmissionId), new { severity = "Low" });
        alfa.Admin.GetResponse<SeverityResponse>(SeverityUri(er, bobSubmissionId)).Severity.Should().Be("Urgent");
    }

    [Test]
    public void SeverityCanChangeInAnArchivedRound()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{data.ElectionRoundId}:archive", new { });

        var response = Put(admin, SeverityUri(data.ElectionRoundId, aliceSubmissionId), new { severity = "High" });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        admin.GetResponse<SeverityResponse>(SeverityUri(data.ElectionRoundId, aliceSubmissionId)).Severity
            .Should().Be("High");
    }

    [Test]
    public void PlatformAdminsCannotSetSeverity()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();

        var response = Put(data.PlatformAdmin, SeverityUri(data.ElectionRoundId, aliceSubmissionId),
            new { severity = "High" });

        response.IsSuccessStatusCode.Should().BeFalse();
    }
}
