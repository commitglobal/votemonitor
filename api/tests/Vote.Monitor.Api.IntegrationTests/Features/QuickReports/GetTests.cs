using System.Net;
using Feature.QuickReports.Get;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.QuickReports;

using static ApiTesting;

public class GetTests : BaseApiTestFixture
{
    [Test]
    public void ShouldAnonymizedCoalitionMembersQuickReports_WhenCoalitionLeader()
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
        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;
        
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
        var aliceQuickReport = alfaNgoAdmin
            .GetResponse<QuickReportDetailedModel>(
                $"/api/election-rounds/{electionRoundId}/quick-reports/{aliceQuickReportId}");  
        
        var bobQuickReport = alfaNgoAdmin
            .GetResponse<QuickReportDetailedModel>(
                $"/api/election-rounds/{electionRoundId}/quick-reports/{bobQuickReportId}");

        // Assert
        aliceQuickReport.Should().NotBeNull();
        aliceQuickReport.MonitoringObserverId.Should().Be(alice.MonitoringObserverId);
        aliceQuickReport.ObserverName.Should().Be(alice.DisplayName);
        aliceQuickReport.Email.Should().Be(alice.Email);
        aliceQuickReport.PhoneNumber.Should().Be(alice.PhoneNumber);

        bobQuickReport.Should().NotBeNull();
        bobQuickReport.MonitoringObserverId.Should().Be(bob.MonitoringObserverId);
        bobQuickReport.ObserverName.Should().Be(bob.MonitoringObserverId.ToString());
        bobQuickReport.Email.Should().Be(bob.MonitoringObserverId.ToString());
        bobQuickReport.PhoneNumber.Should().Be(bob.MonitoringObserverId.ToString());
    }

    [Test]
    public void ShouldReturnNgoQuickReports_WhenCoalitionMember()
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
        var bobQuickReport = betaNgoAdmin
            .GetResponse<QuickReportDetailedModel>(
                $"/api/election-rounds/{electionRoundId}/quick-reports/{bobQuickReportId}");

        // Assert
        bobQuickReport.Should().NotBeNull();
        bobQuickReport.MonitoringObserverId.Should().Be(bob.MonitoringObserverId);
        bobQuickReport.ObserverName.Should().Be(bob.DisplayName);
    }

    [Test]
    public async Task ShouldNotReturnResponses_WhenCoalitionMemberAccessesOtherMembersQuickReports()
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
        
        var aliceQuickReportId =
            scenarioData.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);

        // Act
        var aliceQuickReportResponse = await betaNgoAdmin
            .GetAsync($"/api/election-rounds/{electionRoundId}/quick-reports/{aliceQuickReportId}");  
        
        // Assert
        aliceQuickReportResponse.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
    
    [Test]
    public void ShouldReturnQuickReport_WhenOwnerAccessesIt()
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

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);
        
        var quickReportId =
            scenarioData.ElectionRound.GetQuickReportId(ScenarioObserver.Bob, ScenarioPollingStation.Iasi);

        // Act
        var bobQuickReport = bob.Client
            .GetResponse<QuickReportDetailedModel>(
                $"/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}");

        // Assert
        bobQuickReport.Should().NotBeNull();
        bobQuickReport.MonitoringObserverId.Should().BeEmpty();
        bobQuickReport.ObserverName.Should().BeNull();
        bobQuickReport.Tags.Should().BeEmpty();
        bobQuickReport.Email.Should().BeNull();
        bobQuickReport.PhoneNumber.Should().BeNull();
    }
    
    [Test]
    public async Task ShouldNotReturnResponses_WhenObserverAccessesOtherObserversQuickReport()
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
        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);
        
        var aliceQuickReportId =
            scenarioData.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);

        // Act
        var aliceQuickReportResponse = await bob.Client
            .GetAsync($"/api/election-rounds/{electionRoundId}/quick-reports/{aliceQuickReportId}");  
        
        // Assert
        aliceQuickReportResponse.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

}
