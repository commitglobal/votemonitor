using Feature.Form.Submissions.ListEntries;
using NSubstitute;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Fakers;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Api.IntegrationTests.TestCases;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

public class ListEntriesTests : BaseApiTestFixture
{
    private readonly DateTime _now = DateTime.UtcNow.AddDays(1000);
    private ScenarioData _scenarioData;
    private Guid _electionRoundId;
    private Guid _alfaFormId;
    private Guid _coalitionFormId;
    private Guid _psIasiId;
    private Guid _psBacauId;
    private Guid _psClujId;
    private List<BaseQuestionRequest> _alfaFormQuestions;
    private List<BaseQuestionRequest> _coalitionFormQuestions;

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

        var firstSubmissionAt = _now.AddDays(-5);
        var secondSubmissionAt = _now.AddDays(-3);
        var thirdSubmissionAt = _now.AddDays(-1);

        ApiTimeProvider.UtcNow
            .Returns(firstSubmissionAt, secondSubmissionAt, thirdSubmissionAt);

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
    public void ShouldNotIncludeCoalitionMembersResponses_WhenGettingSubmissionsAsCoalitionLeader_And_DataSourceNgo()
    {
        // Arrange
        var iasiSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psIasiId, _coalitionFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(_alfaFormId, _psClujId, _alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psBacauId, _coalitionFormQuestions).Generate();

        var alice = _scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        var bob = _scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        var firstSubmission = alice.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            clujSubmission);

        var secondSubmission = alice.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            iasiSubmission);

        var thirdSubmission = bob.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFormSubmissions = _scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{_electionRoundId}/form-submissions:byEntry?dataSource=Ngo");

        // Assert
        alfaNgoFormSubmissions.Items
            .Select(x => x.SubmissionId)
            .Should()
            .HaveCount(2)
            .And.BeEquivalentTo([firstSubmission.Id, secondSubmission.Id]);
    }

    [Test]
    public void ShouldIncludeAnonymizedCoalitionMembersResponses_WhenGettingSubmissionsAsCoalitionLeader_And_DataSourceCoalition()
    {
        // Arrange
        var iasiSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psIasiId, _coalitionFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(_alfaFormId, _psClujId, _alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psBacauId, _coalitionFormQuestions).Generate();

        var alice = _scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        var bob = _scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        var firstSubmission = alice.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            clujSubmission);

        var secondSubmission = alice.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            iasiSubmission);

        var thirdSubmission = bob.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFormSubmissions = _scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{_electionRoundId}/form-submissions:byEntry?dataSource=Coalition");

        // Assert
        alfaNgoFormSubmissions.Items
            .Select(x => x.SubmissionId)
            .Should()
            .HaveCount(3)
            .And.BeEquivalentTo([firstSubmission.Id, secondSubmission.Id, thirdSubmission.Id]);

        alfaNgoFormSubmissions.Items.Select(x => x.ObserverName).Should()
            .BeEquivalentTo(alice.FullName, alice.FullName, bob.MonitoringObserverId.ToString());
        alfaNgoFormSubmissions.Items.Select(x => x.Email).Should()
            .BeEquivalentTo(alice.Email, alice.Email, bob.MonitoringObserverId.ToString());
        alfaNgoFormSubmissions.Items.Select(x => x.PhoneNumber).Should()
            .BeEquivalentTo(alice.PhoneNumber, alice.PhoneNumber, bob.MonitoringObserverId.ToString());
    }

    [TestCaseSource(typeof(DataSourcesTestCases))]
    public void ShouldAlwaysGetOnlyNgoResponses_WhenGettingSubmissionsAsCoalitionMember(DataSource dataSource)
    {
        // Arrange
        var iasiSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psIasiId, _coalitionFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(_alfaFormId, _psClujId, _alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(_coalitionFormId, _psBacauId, _coalitionFormQuestions).Generate();

        var alice = _scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = _scenarioData.ObserverByName(ScenarioObserver.Bob);

        var firstSubmission = alice.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            clujSubmission);

        var secondSubmission = bob.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            iasiSubmission);

        var thirdSubmission = bob.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var betaNgoFormSubmissions = _scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{_electionRoundId}/form-submissions:byEntry?dataSource={dataSource}");

        // Assert
        betaNgoFormSubmissions.Items
            .Select(x => x.SubmissionId)
            .Should()
            .HaveCount(2)
            .And.BeEquivalentTo([secondSubmission.Id, thirdSubmission.Id]);
    }
}
