using NSubstitute;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Fakers;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Api.IntegrationTests.TestCases;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;
using GetFiltersResponse = Feature.Form.Submissions.GetFilters.Response;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

public class GetFiltersTests : BaseApiTestFixture
{
    private readonly DateTime _now = DateTime.UtcNow.AddDays(1000);

    private ScenarioData _scenarioData;
    private Guid _electionRoundId;
    
    private HttpClient _alfaNgoAdmin;
    private HttpClient _betaNgoAdmin;
    
    private Guid _alfaFormId;
    private Guid _coalitionFormId;

    private Guid _psIasiId;
    private Guid _psBacauId;
    private Guid _psClujId;

    private List<BaseQuestionRequest> _alfaFormQuestions;
    private List<BaseQuestionRequest> _coalitionFormQuestions;

    private DateTime _firstSubmissionAt;
    private DateTime _secondSubmissionAt;
    private DateTime _thirdSubmissionAt;
    
    [SetUp]
    public void Setup()
    {
        _scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithForm("A", form => form.Publish()))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta])
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                ))
            .Please();

        _alfaNgoAdmin = _scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;
        _betaNgoAdmin = _scenarioData.NgoByName(ScenarioNgos.Beta).Admin;

        _firstSubmissionAt = _now.AddDays(-5);
        _secondSubmissionAt = _now.AddDays(-3);
        _thirdSubmissionAt = _now.AddDays(-1);

        ApiTimeProvider.UtcNow
            .Returns(_firstSubmissionAt, _secondSubmissionAt, _thirdSubmissionAt);

        _electionRoundId = _scenarioData.ElectionRoundId;
        _alfaFormId = _scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;
        _coalitionFormId = _scenarioData.ElectionRound.Coalition.FormId;

        _psIasiId = _scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        _psBacauId = _scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        _psClujId = _scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        _alfaFormQuestions = _scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).Form.Questions;
        _coalitionFormQuestions = _scenarioData.ElectionRound.Coalition.Form.Questions;
    }

    [Test]
    public void ShouldIncludeCoalitionMembersResponses_WhenGettingFiltersAsCoalitionLeader_And_DataSourceCoalition()
    {
        // Arrange
        var iasiSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psIasiId, _coalitionFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(_alfaFormId, _psClujId, _alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psBacauId, _coalitionFormQuestions).Generate();

        var alice = _scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = _scenarioData.ObserverByName(ScenarioObserver.Bob);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            clujSubmission);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            iasiSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFilters = _alfaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{_electionRoundId}/form-submissions:filters?dataSource=Coalition");

        // Assert
        alfaNgoFilters.FormFilterOptions
            .Select(x => x.FormId)
            .Should()
            .HaveCount(2)
            .And
            .BeEquivalentTo([_alfaFormId, _coalitionFormId]);

        alfaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_firstSubmissionAt, TimeSpan.FromMicroseconds(100));
        alfaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_thirdSubmissionAt, TimeSpan.FromMicroseconds(100));
    }

    [Test]
    public void ShouldNotIncludeCoalitionMembersResponses_WhenGettingFiltersAsCoalitionLeader_And_DataSourceNgo()
    {
        // Arrange
        var iasiSubmission =
            new FormSubmissionRequestFaker(_alfaFormId, _psIasiId, _alfaFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(_alfaFormId, _psClujId, _alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psBacauId, _coalitionFormQuestions).Generate();

        var alice = _scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = _scenarioData.ObserverByName(ScenarioObserver.Bob);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            iasiSubmission);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            clujSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFilters = _alfaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{_electionRoundId}/form-submissions:filters?dataSource=Ngo");

        // Assert
        alfaNgoFilters.FormFilterOptions
            .Select(x => x.FormId)
            .Should()
            .HaveCount(1)
            .And
            .BeEquivalentTo([_alfaFormId]);

        alfaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_firstSubmissionAt, TimeSpan.FromMicroseconds(100));
        alfaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_secondSubmissionAt, TimeSpan.FromMicroseconds(100));
    }

    [TestCaseSource(typeof(DataSourcesTestCases))]
    public void ShouldIncludeOnlyNgoResponses_WhenGettingFiltersAsCoalitionMember(DataSource dataSource)
    {
        // Arrange
        var iasiSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psIasiId, _coalitionFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(_alfaFormId, _psClujId, _alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psBacauId, _coalitionFormQuestions).Generate();

        var alice = _scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = _scenarioData.ObserverByName(ScenarioObserver.Bob);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            clujSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            iasiSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var betaNgoFilters = _betaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{_electionRoundId}/form-submissions:filters?dataSource={dataSource}");

        // Assert
        betaNgoFilters.FormFilterOptions
            .Select(x => x.FormId)
            .Should()
            .HaveCount(1)
            .And
            .BeEquivalentTo([_coalitionFormId]);

        betaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_secondSubmissionAt, TimeSpan.FromMicroseconds(100));
        betaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_thirdSubmissionAt, TimeSpan.FromMicroseconds(100));
    }
}
