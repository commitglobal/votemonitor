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
    private static readonly DateTime _now = DateTime.UtcNow.AddDays(1000);
    private readonly DateTime _firstSubmissionAt = _now.AddDays(-5);
    private readonly DateTime _secondSubmissionAt = _now.AddDays(-3);
    private readonly DateTime _thirdSubmissionAt = _now.AddDays(-1);

    [Test]
    public void ShouldNotIncludeCoalitionMembersResponses_WhenGettingSubmissionsAsCoalitionLeader_And_DataSourceNgo()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
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

        ApiTimeProvider.UtcNow
            .Returns(_firstSubmissionAt, _secondSubmissionAt, _thirdSubmissionAt);

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;
        var coalitionFormId = scenarioData.ElectionRound.Coalition.FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).Form.Questions;
        var coalitionFormQuestions = scenarioData.ElectionRound.Coalition.Form.Questions;

        var iasiSubmission =
            new FormSubmissionRequestFaker(coalitionFormId, psIasiId, coalitionFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        var firstSubmission = alice.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        var secondSubmission = alice.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        var thirdSubmission = bob.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource=Ngo");

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
        var scenarioData = ScenarioBuilder.New(CreateClient)
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

        ApiTimeProvider.UtcNow
            .Returns(_firstSubmissionAt, _secondSubmissionAt, _thirdSubmissionAt);

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;
        var coalitionFormId = scenarioData.ElectionRound.Coalition.FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).Form.Questions;
        var coalitionFormQuestions = scenarioData.ElectionRound.Coalition.Form.Questions;

        var iasiSubmission =
            new FormSubmissionRequestFaker(coalitionFormId, psIasiId, coalitionFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        var firstSubmission = alice.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        var secondSubmission = alice.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        var thirdSubmission = bob.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource=Coalition");

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
        var scenarioData = ScenarioBuilder.New(CreateClient)
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

        ApiTimeProvider.UtcNow
            .Returns(_firstSubmissionAt, _secondSubmissionAt, _thirdSubmissionAt);

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;
        var coalitionFormId = scenarioData.ElectionRound.Coalition.FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).Form.Questions;
        var coalitionFormQuestions = scenarioData.ElectionRound.Coalition.Form.Questions;
        var iasiSubmission =
            new FormSubmissionRequestFaker(coalitionFormId, psIasiId, coalitionFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        var firstSubmission = alice.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        var secondSubmission = bob.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        var thirdSubmission = bob.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var betaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource={dataSource}");

        // Assert
        betaNgoFormSubmissions.Items
            .Select(x => x.SubmissionId)
            .Should()
            .HaveCount(2)
            .And.BeEquivalentTo([secondSubmission.Id, thirdSubmission.Id]);
    }
    
    [TestCaseSource(typeof(DataSourcesTestCases))]
    public void ShouldAGetOnlyNgoResponses_WhenGettingSubmissions_AsIndependentNgo(DataSource dataSource)
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa,
                    ngo => ngo.WithForm("A", form => form.Publish()).WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta,
                    ngo => ngo.WithForm("A", form => form.Publish()).WithMonitoringObserver(ScenarioObserver.Bob)))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;
        var betaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Beta).FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).Form.Questions;
        var betaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Beta).Form.Questions;

        var iasiSubmission =
            new FormSubmissionRequestFaker(alfaFormId, psIasiId, alfaFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(betaFormId, psBacauId, betaFormQuestions).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        var firstSubmission = alice.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        var secondSubmission = alice.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        var thirdSubmission = bob.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource={dataSource}");

        var betaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource={dataSource}");

        // Assert
        alfaNgoFormSubmissions.Items
            .Select(x => x.SubmissionId)
            .Should()
            .HaveCount(2)
            .And.BeEquivalentTo([firstSubmission.Id, secondSubmission.Id]);

        betaNgoFormSubmissions.Items
            .Select(x => x.SubmissionId)
            .Should()
            .HaveCount(1)
            .And.BeEquivalentTo([thirdSubmission.Id]);
    }
}
