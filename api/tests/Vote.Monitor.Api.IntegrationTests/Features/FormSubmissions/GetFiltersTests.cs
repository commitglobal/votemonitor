using NSubstitute;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Fakers;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Api.IntegrationTests.TestCases;
using Vote.Monitor.Core.Models;
using GetFiltersResponse = Feature.Form.Submissions.GetFilters.Response;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

public class GetFiltersTests : BaseApiTestFixture
{
    private static readonly DateTime _now = DateTime.UtcNow.AddDays(1000);
    private readonly DateTime _firstSubmissionAt = _now.AddDays(-5);
    private readonly DateTime _secondSubmissionAt = _now.AddDays(-3);
    private readonly DateTime _thirdSubmissionAt = _now.AddDays(-1);

    [Test]
    public void ShouldIncludeCoalitionMembersResponses_WhenGettingFiltersAsCoalitionLeader_And_DataSourceCoalition()
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
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta])
                    .WithForm("A", [ScenarioNgos.Alfa])
                ))
            .Please();

        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;

        ApiTimeProvider.UtcNow
            .Returns(_firstSubmissionAt, _secondSubmissionAt, _thirdSubmissionAt);

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.Coalition.FormByCode("A").Id;
        var coalitionFormId = scenarioData.ElectionRound.Coalition.FormByCode("Shared").Id;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.Coalition.FormByCode("A").Questions;
        var coalitionFormQuestions = scenarioData.ElectionRound.Coalition.FormByCode("Shared").Questions;

        var iasiSubmission =
            new FakeSubmission(coalitionFormId, psIasiId, coalitionFormQuestions).Generate();
        var clujSubmission = new FakeSubmission(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new FakeSubmission(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFilters = alfaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:filters?dataSource=Coalition");

        // Assert
        alfaNgoFilters.FormFilterOptions
            .Select(x => x.FormId)
            .Should()
            .HaveCount(2)
            .And
            .BeEquivalentTo([alfaFormId, coalitionFormId]);

        alfaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_firstSubmissionAt, TimeSpan.FromMicroseconds(100));
        alfaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_thirdSubmissionAt, TimeSpan.FromMicroseconds(100));
    }

    [Test]
    public void
        ShouldIncludeCoalitionMembersResponses_AndIgnoreMembersOwnFormsSubmissions_WhenGettingFiltersAsCoalitionLeader_And_DataSourceCoalition()
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
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithForm("A"))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta])
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                ))
            .Please();

        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;

        ApiTimeProvider.UtcNow
            .Returns(_firstSubmissionAt, _secondSubmissionAt, _thirdSubmissionAt);

        var electionRoundId = scenarioData.ElectionRoundId;
        var betaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Beta).FormId;
        var coalitionFormId = scenarioData.ElectionRound.Coalition.FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var betaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Beta).Form.Questions;
        var coalitionFormQuestions = scenarioData.ElectionRound.Coalition.Form.Questions;

        var iasiSubmission = new FakeSubmission(betaFormId, psIasiId, betaFormQuestions).Generate();
        var clujSubmission = new FakeSubmission(betaFormId, psClujId, betaFormQuestions).Generate();
        var bacauSubmission =
            new FakeSubmission(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFilters = alfaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:filters?dataSource=Coalition");

        // Assert
        alfaNgoFilters.FormFilterOptions
            .Select(x => x.FormId)
            .Should()
            .HaveCount(1)
            .And
            .BeEquivalentTo([coalitionFormId]);

        alfaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_thirdSubmissionAt, TimeSpan.FromMicroseconds(100));

        alfaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_thirdSubmissionAt, TimeSpan.FromMicroseconds(100));
    }

    [Test]
    public void ShouldNotIncludeCoalitionMembersResponses_WhenGettingFiltersAsCoalitionLeader_And_DataSourceNgo()
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
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta])
                    .WithForm("A", [ScenarioNgos.Alfa])
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                ))
            .Please();

        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;

        ApiTimeProvider.UtcNow
            .Returns(_firstSubmissionAt, _secondSubmissionAt, _thirdSubmissionAt);

        var electionRoundId = scenarioData.ElectionRoundId;

        var alfaFormId = scenarioData.ElectionRound.Coalition.FormByCode("A").Id;
        var coalitionFormId = scenarioData.ElectionRound.Coalition.FormByCode("Shared").Id;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.Coalition.FormByCode("A").Questions;
        var coalitionFormQuestions = scenarioData.ElectionRound.Coalition.FormByCode("Shared").Questions;

        var iasiSubmission =
            new FakeSubmission(alfaFormId, psIasiId, alfaFormQuestions).Generate();
        var clujSubmission = new FakeSubmission(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new FakeSubmission(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFilters = alfaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:filters?dataSource=Ngo");

        // Assert
        alfaNgoFilters.FormFilterOptions
            .Select(x => x.FormId)
            .Should()
            .HaveCount(1)
            .And
            .BeEquivalentTo([alfaFormId]);

        alfaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_firstSubmissionAt, TimeSpan.FromMicroseconds(100));
        alfaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_secondSubmissionAt, TimeSpan.FromMicroseconds(100));
    }

    [TestCaseSource(typeof(DataSourcesTestCases))]
    public void ShouldIncludeOnlyNgoResponses_WhenGettingFiltersAsCoalitionMember(DataSource dataSource)
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
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithForm("A"))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta])
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                ))
            .Please();

        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;
        var betaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Beta).Admin;

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
            new FakeSubmission(coalitionFormId, psIasiId, coalitionFormQuestions).Generate();
        var clujSubmission = new FakeSubmission(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new FakeSubmission(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var betaNgoFilters = betaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:filters?dataSource={dataSource}");

        // Assert
        betaNgoFilters.FormFilterOptions
            .Select(x => x.FormId)
            .Should()
            .HaveCount(1)
            .And
            .BeEquivalentTo([coalitionFormId]);

        betaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_secondSubmissionAt, TimeSpan.FromMicroseconds(100));
        betaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_thirdSubmissionAt, TimeSpan.FromMicroseconds(100));
    }


    [TestCaseSource(typeof(DataSourcesTestCases))]
    public void ShouldIncludeOnlyNgoResponses_WhenGettingFiltersAsIndependentNgo(DataSource dataSource)
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
                    ngo => ngo.WithForm("A").WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta,
                    ngo => ngo.WithForm("A").WithMonitoringObserver(ScenarioObserver.Bob)))
            .Please();

        ApiTimeProvider.UtcNow
            .Returns(_firstSubmissionAt, _secondSubmissionAt, _thirdSubmissionAt);
        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;
        var betaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Beta).FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).Form.Questions;
        var betaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Beta).Form.Questions;

        var iasiSubmission =
            new FakeSubmission(alfaFormId, psIasiId, alfaFormQuestions).Generate();
        var clujSubmission = new FakeSubmission(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new FakeSubmission(betaFormId, psBacauId, betaFormQuestions).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        alice.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        alice.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        bob.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;
        var betaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Beta).Admin;

        // Act
        var aflaNgoFilters = alfaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:filters?dataSource={dataSource}");

        // Assert
        aflaNgoFilters.FormFilterOptions
            .Select(x => x.FormId)
            .Should()
            .HaveCount(1)
            .And
            .BeEquivalentTo([scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgo.Alfa).FormId]);

        aflaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_firstSubmissionAt, TimeSpan.FromMicroseconds(100));
        aflaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_secondSubmissionAt, TimeSpan.FromMicroseconds(100));
    }
}
