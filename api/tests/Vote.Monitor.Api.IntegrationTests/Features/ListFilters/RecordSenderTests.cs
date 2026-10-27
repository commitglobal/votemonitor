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
    public void CorrectableRecord_ShouldShowObserverNameEmailAndSubmissionNumber()
    {
        var submission = _scenarioData.PlatformAdmin.GetResponse<JsonElement>(
            $"{ElectionRoundUrl}/polling-station-corrections:records/FormSubmission/{_iasiSecondId}");
        submission.GetProperty("observerName").GetString().Should().Be(Alice.DisplayName);
        submission.GetProperty("observerEmail").GetString().Should().Be(Alice.Email);
        SubmissionNumber(submission).Should().Be(2);

        var quickReport = _scenarioData.PlatformAdmin.GetResponse<JsonElement>(
            $"{ElectionRoundUrl}/polling-station-corrections:records/QuickReport/{_quickReportId}");
        quickReport.GetProperty("observerEmail").GetString().Should().Be(Alice.Email);
        SubmissionNumber(quickReport).Should().BeNull();
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

    private List<JsonElement> Rows(string url) =>
        _scenarioData.PlatformAdmin.GetResponse<PagedResponse<JsonElement>>(url).Items;

    private static Guid Id(JsonElement row) => row.GetProperty("id").GetGuid();

    private static int? SubmissionNumber(JsonElement row) =>
        row.TryGetProperty("submissionNumber", out var value) && value.ValueKind == JsonValueKind.Number
            ? value.GetInt32()
            : null;
}
