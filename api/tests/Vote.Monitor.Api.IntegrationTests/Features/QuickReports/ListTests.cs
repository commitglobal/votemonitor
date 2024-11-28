using Feature.QuickReports.List;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Api.IntegrationTests.TestCases;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.Features.QuickReports;

using static ApiTesting;

public class ListTests : BaseApiTestFixture
{
    [Test]
    public void ShouldExcludeCoalitionMembersResponses_WhenCoalitionLeader_And_DataSourceNgo()
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
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                    .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                    .WithQuickReport(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                ))
            .Please();

        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;
        var electionRoundId = scenarioData.ElectionRoundId;

        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        var aliceQuickReportId =
            scenarioData.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);

        // Act
        var alfaNgoQuickReports = alfaNgoAdmin
            .GetResponse<PagedResponse<QuickReportOverviewModel>>(
                $"/api/election-rounds/{electionRoundId}/quick-reports?dataSource=Ngo");

        // Assert
        alfaNgoQuickReports.Should().NotBeNull();
        alfaNgoQuickReports.Items.Should().HaveCount(1);
        
        var aliceQuickReport = alfaNgoQuickReports.Items.First();
        aliceQuickReport.Id.Should().Be(aliceQuickReportId);
        aliceQuickReport.MonitoringObserverId.Should().Be(alice.MonitoringObserverId);
        aliceQuickReport.ObserverName.Should().Be(alice.DisplayName);
        aliceQuickReport.PhoneNumber.Should().Be(alice.PhoneNumber);
        aliceQuickReport.Email.Should().Be(alice.Email);
    }

    [Test]
    public void ShouldAnonymizedCoalitionMembersResponses_WhenCoalitionLeader_And_DataSourceCoalition()
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
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                    .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                    .WithQuickReport(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                ))
            .Please();

        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;
        var electionRoundId = scenarioData.ElectionRoundId;

        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        var aliceQuickReportId =
            scenarioData.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var bobQuickReportId =
            scenarioData.ElectionRound.GetQuickReportId(ScenarioObserver.Bob, ScenarioPollingStation.Iasi);

        // Act
        var coalitionQuickReports = alfaNgoAdmin
            .GetResponse<PagedResponse<QuickReportOverviewModel>>(
                $"/api/election-rounds/{electionRoundId}/quick-reports?dataSource=Coalition");

        // Assert
        coalitionQuickReports.Should().NotBeNull();
        coalitionQuickReports.Items.Should().HaveCount(2);

        var aliceQuickReport = coalitionQuickReports.Items.First(x => x.Id == aliceQuickReportId);
        aliceQuickReport.MonitoringObserverId.Should().Be(alice.MonitoringObserverId);
        aliceQuickReport.ObserverName.Should().Be(alice.DisplayName);
        aliceQuickReport.PhoneNumber.Should().Be(alice.PhoneNumber);
        aliceQuickReport.Email.Should().Be(alice.Email);

        var bobQuickReport = coalitionQuickReports.Items.First(x => x.Id == bobQuickReportId);
        bobQuickReport.MonitoringObserverId.Should().Be(bob.MonitoringObserverId);
        bobQuickReport.ObserverName.Should().Be(bob.MonitoringObserverId.ToString());
        bobQuickReport.PhoneNumber.Should().Be(bob.MonitoringObserverId.ToString());
        bobQuickReport.Email.Should().Be(bob.MonitoringObserverId.ToString());
    }

    [TestCaseSource(typeof(DataSourcesTestCases))]
    public void ShouldReturnNgoResponses_WhenCoalitionMember(DataSource dataSource)
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
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                    .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                    .WithQuickReport(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                ))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var betaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Beta).Admin;

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        var bobQuickReportId =
            scenarioData.ElectionRound.GetQuickReportId(ScenarioObserver.Bob, ScenarioPollingStation.Iasi);

        // Act
        var coalitionQuickReports = betaNgoAdmin
            .GetResponse<PagedResponse<QuickReportOverviewModel>>(
                $"/api/election-rounds/{electionRoundId}/quick-reports?dataSource={dataSource}");

        // Assert
        coalitionQuickReports.Should().NotBeNull();
        coalitionQuickReports.Items.Should().ContainSingle();

        var bobQuickReport = coalitionQuickReports.Items.First();
        bobQuickReport.Id.Should().Be(bobQuickReportId);
        bobQuickReport.MonitoringObserverId.Should().Be(bob.MonitoringObserverId);
        bobQuickReport.ObserverName.Should().Be(bob.DisplayName);
        bobQuickReport.PhoneNumber.Should().Be(bob.PhoneNumber);
        bobQuickReport.Email.Should().Be(bob.Email);
    }

    [Test]
    public void ShouldAllowFilteringResponsesByNgoId_WhenCoalitionLeader_And_DataSourceCoalition()
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
                    .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                    .WithQuickReport(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                    .WithQuickReport(ScenarioObserver.Bob, ScenarioPollingStation.Cluj)
                    .WithQuickReport(ScenarioObserver.Bob, ScenarioPollingStation.Bacau)
                ))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);
        var betaNgoId = scenarioData.NgoIdByName(ScenarioNgos.Beta);

        // Act
        var coalitionQuickReports = alfaNgoAdmin
            .GetResponse<PagedResponse<QuickReportOverviewModel>>(
                $"/api/election-rounds/{electionRoundId}/quick-reports?dataSource=Coalition&coalitionMemberId={betaNgoId}");

        // Assert
        coalitionQuickReports.Should().NotBeNull();
        coalitionQuickReports.TotalCount.Should().Be(3);
        coalitionQuickReports.Items
            .Should()
            .HaveCount(3)
            .And
            .AllSatisfy(qr => qr.MonitoringObserverId.Should().Be(bob.MonitoringObserverId));
    }
}
