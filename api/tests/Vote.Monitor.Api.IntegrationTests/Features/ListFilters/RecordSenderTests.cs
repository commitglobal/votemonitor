using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Api.IntegrationTests.Models;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.Features.ListFilters;

using static ApiTesting;

/// <summary>
/// Platform admins correcting polling stations see who sent a record (observer name and email) and, for form
/// submissions, which of the observer's submissions of that form at that polling station it is.
/// </summary>
public class RecordSenderTests : BaseApiTestFixture
{
    private ScenarioData _scenarioData = null!;
    private Guid _iasiFirstId;
    private Guid _iasiSecondId;
    private Guid _bacauSubmissionId;
    private Guid _quickReportId;

    private string ElectionRoundUrl => $"/api/election-rounds/{_scenarioData.ElectionRoundId}";

    private (Guid ObserverId, Guid MonitoringObserverId, HttpClient Client, string DisplayName, string Email, string
        PhoneNumber) Alice => _scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa)
        .ObserverByName(ScenarioObserver.Alice);

    /// <summary>
    /// Alice (Alfa) sent form A twice in Iasi (the older one posted last) and once in Bacau, and a quick report in Bacau.
    /// </summary>
    [SetUp]
    public void CreateScenario()
    {
        _scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi, level1: "Iasi", number: "01/001")
                .WithPollingStation(ScenarioPollingStation.Bacau, level1: "Bacau", number: "01/002")
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithForm("A"))
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Bacau))
            .Please();

        var start = new DateTime(2026, 10, 1, 8, 0, 0, DateTimeKind.Utc);
        _iasiSecondId = Submit(ScenarioPollingStation.Iasi, start.AddHours(2));
        _iasiFirstId = Submit(ScenarioPollingStation.Iasi, start);
        _bacauSubmissionId = Submit(ScenarioPollingStation.Bacau, start.AddHours(1));
        _quickReportId = _scenarioData.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Bacau);
    }

    private Guid Submit(ScenarioPollingStation pollingStation, DateTime createdAt)
    {
        var id = Guid.NewGuid();
        Alice.Client.PostWithResponse<JsonElement>($"{ElectionRoundUrl}/form-submissions/{id}", new
        {
            PollingStationId = _scenarioData.ElectionRound.PollingStationByName(pollingStation),
            FormId = _scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId,
            Answers = Array.Empty<object>(),
            CreatedAt = createdAt,
            LastUpdatedAt = createdAt
        });

        return id;
    }

    private Dictionary<Guid, int?> ExpectedNumbers => new()
    {
        [_iasiFirstId] = 1,
        [_iasiSecondId] = 2,
        [_bacauSubmissionId] = 1,
        [_quickReportId] = null
    };

    [Test]
    public void CorrectableRecords_ShouldShowObserverNameEmailAndSubmissionNumber()
    {
        var rows = Rows($"{ElectionRoundUrl}/polling-station-corrections:records?pageSize=100");

        rows.Should().HaveCount(4);
        foreach (var row in rows)
        {
            row.GetProperty("observerName").GetString().Should().Be(Alice.DisplayName);
            row.GetProperty("observerEmail").GetString().Should().Be(Alice.Email);
            SubmissionNumber(row).Should().Be(ExpectedNumbers[Id(row)]);
        }
    }

    [Test]
    public void PlatformAdmins_GetRecordsFromTheirOwnGets_WithoutWhatWasReported()
    {
        var platform = _scenarioData.PlatformAdmin;
        var ngoAdmin = _scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;

        // Form submission: who sent it, the NGO, where and which of Alice's submissions it is; nothing reported.
        var submissionUri = $"{ElectionRoundUrl}/form-submissions/{_iasiSecondId}:v2";
        var submission = platform.GetResponse<JsonElement>(submissionUri);
        submission.GetProperty("observerName").GetString().Should().Be(Alice.DisplayName);
        submission.GetProperty("email").GetString().Should().Be(Alice.Email);
        submission.GetProperty("ngoName").GetString().Should().NotBeNullOrEmpty();
        submission.GetProperty("number").GetString().Should().Be("01/001");
        submission.GetProperty("submissionNumber").GetInt32().Should().Be(2);
        ShouldLeaveOut(submission, "questions", "answers", "notes", "attachments", "phoneNumber", "tags", "breaks",
            "numberOfFlaggedAnswers", "numberOfQuestionsAnswered", "followUpStatus", "reviewStage", "severity", "labels");
        var ngoSubmission = ngoAdmin.GetResponse<JsonElement>(submissionUri);
        ngoSubmission.TryGetProperty("answers", out _).Should().BeTrue();
        ngoSubmission.TryGetProperty("reviewStage", out _).Should().BeTrue();

        // Quick report: the NGO is named; title, description, category and media are left out.
        var quickReportUri = $"{ElectionRoundUrl}/quick-reports/{_quickReportId}";
        var quickReport = platform.GetResponse<JsonElement>(quickReportUri);
        quickReport.GetProperty("email").GetString().Should().Be(Alice.Email);
        quickReport.GetProperty("ngoName").GetString().Should().NotBeNullOrEmpty();
        ShouldLeaveOut(quickReport, "title", "description", "incidentCategory", "pollingStationDetails", "attachments",
            "phoneNumber", "tags", "followUpStatus", "reviewStage", "severity", "labels");
        ngoAdmin.GetResponse<JsonElement>(quickReportUri).TryGetProperty("title", out _).Should().BeTrue();

        // Incident report: answers, notes, media and the location's description are left out.
        var incidentForm = Dummy.Form("INC");
        incidentForm.FormType = FormType.IncidentReporting;
        var form = ngoAdmin.PostWithResponse<CreateFormRequest>($"{ElectionRoundUrl}/forms", incidentForm);
        ngoAdmin.PostWithoutResponse($"{ElectionRoundUrl}/forms/{form.Id}:publish");
        var incidentReportId = Guid.NewGuid();
        Alice.Client.PostAsJsonAsync($"{ElectionRoundUrl}/incident-reports",
                new
                {
                    id = incidentReportId, formId = form.Id, locationType = "NotRelatedToAPollingStation",
                    locationDescription = "At the town hall"
                })
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.OK);
        var incidentUri = $"{ElectionRoundUrl}/incident-reports/{incidentReportId}";
        var incident = platform.GetResponse<JsonElement>(incidentUri);
        incident.GetProperty("observerName").GetString().Should().Be(Alice.DisplayName);
        incident.GetProperty("email").GetString().Should().Be(Alice.Email);
        incident.GetProperty("ngoName").GetString().Should().NotBeNullOrEmpty();
        ShouldLeaveOut(incident, "questions", "answers", "notes", "attachments", "locationDescription",
            "numberOfQuestionsAnswered", "followUpStatus", "reviewStage", "severity", "labels");
        ngoAdmin.GetResponse<JsonElement>(incidentUri).GetProperty("locationDescription").GetString()
            .Should().Be("At the town hall");
    }

    private static void ShouldLeaveOut(JsonElement record, params string[] fields)
    {
        foreach (var field in fields)
        {
            (record.TryGetProperty(field, out var value) && value.ValueKind != JsonValueKind.Null).Should()
                .BeFalse($"platform admins mustn't get {field}: {record}");
        }
    }

    [Test]
    public void CorrectableRecords_FiltersAndPagesShouldNotChangeTheSubmissionNumber()
    {
        var url = $"{ElectionRoundUrl}/polling-station-corrections:records";

        // One row per page, newest first: the second Iasi submission, then the first.
        var pages = Enumerable.Range(1, 2)
            .SelectMany(page => Rows($"{url}?level1Filter=Iasi&recordType=FormSubmission&pageSize=1&pageNumber={page}"))
            .ToList();
        pages.Select(Id).Should().Equal(_iasiSecondId, _iasiFirstId);
        pages.Select(SubmissionNumber).Should().Equal(2, 1);

        var oldestFirst = Rows($"{url}?level1Filter=Iasi&sortColumnName=SubmittedAt&sortOrder=Asc&pageSize=1&pageNumber=2");
        oldestFirst.Select(Id).Should().Equal(_iasiSecondId);
        oldestFirst.Select(SubmissionNumber).Should().Equal(2);

        var bacau = Rows($"{url}?level1Filter=Bacau&recordType=FormSubmission");
        bacau.Select(SubmissionNumber).Should().Equal(1);
    }

    [Test]
    public void AllFormSubmissions_ShouldShowObserverNameEmailAndSubmissionNumber()
    {
        var rows = Rows($"{ElectionRoundUrl}/form-submissions:all?pageSize=100");

        rows.Should().HaveCount(3);
        foreach (var row in rows)
        {
            row.GetProperty("observerName").GetString().Should().Be(Alice.DisplayName);
            row.GetProperty("observerEmail").GetString().Should().Be(Alice.Email);
            row.GetProperty("observerPhoneNumber").GetString().Should().NotBe(Alice.PhoneNumber);
            row.GetProperty("submissionNumber").GetInt32()
                .Should().Be(ExpectedNumbers[row.GetProperty("submissionId").GetGuid()]);
        }
    }

    [Test]
    public void AllFormSubmissions_FiltersAndPagesShouldNotChangeTheSubmissionNumber()
    {
        var url = $"{ElectionRoundUrl}/form-submissions:all";

        // One row per page, newest first: the second Iasi submission, then the first.
        var pages = Enumerable.Range(1, 2)
            .SelectMany(page => Rows($"{url}?level1Filter=Iasi&pageSize=1&pageNumber={page}"))
            .ToList();
        pages.Select(row => row.GetProperty("submissionId").GetGuid()).Should().Equal(_iasiSecondId, _iasiFirstId);
        pages.Select(row => row.GetProperty("submissionNumber").GetInt32()).Should().Equal(2, 1);

        var oldestFirst = Rows($"{url}?level1Filter=Iasi&sortColumnName=TimeSubmitted&sortOrder=Asc&pageSize=1&pageNumber=2");
        oldestFirst.Select(row => row.GetProperty("submissionId").GetGuid()).Should().Equal(_iasiSecondId);
        oldestFirst.Select(row => row.GetProperty("submissionNumber").GetInt32()).Should().Equal(2);
    }

    [Test]
    public void SubmissionsList_ShouldNumberAnObserversSubmissionsAtTheSamePollingStation()
    {
        var admin = _scenarioData.AdminOfNgo(ScenarioNgos.Alfa);
        var url = $"{ElectionRoundUrl}/form-submissions:byEntryV2";

        var rows = admin.PostWithResponse<PagedResponse<JsonElement>>(url, new { PageSize = 100 }).Items
            .ToDictionary(row => row.GetProperty("submissionId").GetGuid());

        rows[_iasiFirstId].GetProperty("submissionNumber").GetInt32().Should().Be(1);
        rows[_iasiSecondId].GetProperty("submissionNumber").GetInt32().Should().Be(2);
        rows[_iasiFirstId].GetProperty("submissionCount").GetInt32().Should().Be(2);
        rows[_bacauSubmissionId].GetProperty("submissionNumber").GetInt32().Should().Be(1);
        rows[_bacauSubmissionId].GetProperty("submissionCount").GetInt32().Should().Be(1);

        // Counted over all of the observer's submissions: a page holding only the second keeps its number.
        var page = admin.PostWithResponse<PagedResponse<JsonElement>>(url, new { PageSize = 1, SortColumnName = "TimeSubmitted", SortOrder = "Desc" }).Items;
        page.Single().GetProperty("submissionId").GetGuid().Should().Be(_iasiSecondId);
        page.Single().GetProperty("submissionNumber").GetInt32().Should().Be(2);
    }

    [Test]
    public void EverySubmissionView_ShouldCountTheObserversSubmissionsAtTheSamePollingStation()
    {
        var corrections = Rows($"{ElectionRoundUrl}/polling-station-corrections:records?pageSize=100").ToDictionary(Id);
        corrections[_iasiSecondId].GetProperty("submissionCount").GetInt32().Should().Be(2);
        corrections[_bacauSubmissionId].GetProperty("submissionCount").GetInt32().Should().Be(1);
        // Not a form submission: no count (null values are left out of the response).
        corrections[_quickReportId].TryGetProperty("submissionCount", out var quickReportCount).Should().BeFalse(quickReportCount.ToString());

        var all = Rows($"{ElectionRoundUrl}/form-submissions:all?pageSize=100").ToDictionary(row => row.GetProperty("submissionId").GetGuid());
        all[_iasiFirstId].GetProperty("submissionCount").GetInt32().Should().Be(2);
        all[_bacauSubmissionId].GetProperty("submissionCount").GetInt32().Should().Be(1);

        var admin = _scenarioData.AdminOfNgo(ScenarioNgos.Alfa);
        var second = admin.GetResponse<JsonElement>($"{ElectionRoundUrl}/form-submissions/{_iasiSecondId}:v2");
        second.GetProperty("submissionNumber").GetInt32().Should().Be(2);
        second.GetProperty("submissionCount").GetInt32().Should().Be(2);
        var bacau = admin.GetResponse<JsonElement>($"{ElectionRoundUrl}/form-submissions/{_bacauSubmissionId}:v2");
        bacau.GetProperty("submissionNumber").GetInt32().Should().Be(1);
        bacau.GetProperty("submissionCount").GetInt32().Should().Be(1);
    }

    [Test]
    public void FormsList_ShouldCountEachFormsSubmissions()
    {
        var admin = _scenarioData.AdminOfNgo(ScenarioNgos.Alfa);
        var formId = _scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;

        var forms = admin.GetResponse<List<JsonElement>>($"{ElectionRoundUrl}/forms:listAll");

        forms.Single(form => form.GetProperty("id").GetGuid() == formId)
            .GetProperty("numberOfSubmissions").GetInt32().Should().Be(3);
    }

    private List<JsonElement> Rows(string url) =>
        _scenarioData.PlatformAdmin.GetResponse<PagedResponse<JsonElement>>(url).Items;

    private static Guid Id(JsonElement row) => row.GetProperty("id").GetGuid();

    private static int? SubmissionNumber(JsonElement row) =>
        row.TryGetProperty("submissionNumber", out var value) && value.ValueKind == JsonValueKind.Number
            ? value.GetInt32()
            : null;
}
