using NSubstitute;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Fakers;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Api.IntegrationTests.TestCases;
using Vote.Monitor.Core.Models;
using GetFiltersResponse = Feature.QuickReports.GetFilters.Response;

namespace Vote.Monitor.Api.IntegrationTests.Features.QuickReports;

using static ApiTesting;

public class GetFiltersTests : BaseApiTestFixture
{
    private static readonly DateTime _now = DateTime.UtcNow.AddDays(1000);
    private readonly DateTime _firstQuickReportAt = _now.AddDays(-5);
    private readonly DateTime _secondQuickReportAt = _now.AddDays(-3);
    private readonly DateTime _thirdQuickReportAt = _now.AddDays(-1);

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
                ))
            .Please();

        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;

        ApiTimeProvider.UtcNow
            .Returns(_firstQuickReportAt, _secondQuickReportAt, _thirdQuickReportAt);

        var electionRoundId = scenarioData.ElectionRoundId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var iasiQuickReport = new QuickReportRequestFaker(psIasiId).Generate();
        var clujQuickReport = new QuickReportRequestFaker(psClujId).Generate();
        var bacauQuickReport = new QuickReportRequestFaker(psBacauId).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/quick-reports",
            iasiQuickReport);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/quick-reports",
            clujQuickReport);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/quick-reports",
            bacauQuickReport);

        // Act
        var alfaNgoFilters = alfaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{electionRoundId}/quick-reports:filters?dataSource=Coalition");

        // Assert
        alfaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_firstQuickReportAt, TimeSpan.FromMicroseconds(100));

        alfaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_thirdQuickReportAt, TimeSpan.FromMicroseconds(100));
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
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                ))
            .Please();

        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;

        ApiTimeProvider.UtcNow
            .Returns(_firstQuickReportAt, _secondQuickReportAt, _thirdQuickReportAt);

        var electionRoundId = scenarioData.ElectionRoundId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var iasiQuickReport = new QuickReportRequestFaker(psIasiId).Generate();
        var clujQuickReport = new QuickReportRequestFaker(psClujId).Generate();
        var bacauQuickReport = new QuickReportRequestFaker(psBacauId).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/quick-reports",
            iasiQuickReport);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/quick-reports",
            clujQuickReport);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/quick-reports",
            bacauQuickReport);

        // Act
        var alfaNgoFilters = alfaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{electionRoundId}/quick-reports:filters?dataSource=Ngo");

        // Assert
        alfaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_firstQuickReportAt, TimeSpan.FromMicroseconds(100));

        alfaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_secondQuickReportAt, TimeSpan.FromMicroseconds(100));
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
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                ))
            .Please();

        var betaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Beta).Admin;

        ApiTimeProvider.UtcNow
            .Returns(_firstQuickReportAt, _secondQuickReportAt, _thirdQuickReportAt);

        var electionRoundId = scenarioData.ElectionRoundId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var iasiQuickReport = new QuickReportRequestFaker(psIasiId).Generate();
        var clujQuickReport = new QuickReportRequestFaker(psClujId).Generate();
        var bacauQuickReport = new QuickReportRequestFaker(psBacauId).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/quick-reports",
            iasiQuickReport);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/quick-reports",
            clujQuickReport);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/quick-reports",
            bacauQuickReport);

        // Act
        var betaNgoFilters = betaNgoAdmin
            .GetResponse<GetFiltersResponse>(
                $"/api/election-rounds/{electionRoundId}/quick-reports:filters?dataSource={dataSource}");

        // Assert
        betaNgoFilters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should()
            .BeCloseTo(_thirdQuickReportAt, TimeSpan.FromMicroseconds(100));

        betaNgoFilters.TimestampsFilterOptions.LastSubmissionTimestamp.Should()
            .BeCloseTo(_thirdQuickReportAt, TimeSpan.FromMicroseconds(100));
    }
}
