using Feature.QuickReports.Get;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Vote.Monitor.Api.IntegrationTests.Features.QuickReports;

using static ApiTesting;

public class UpdateStatusTests : BaseApiTestFixture
{
    [Test]
    public void NgoStaffCanUpdateStatusForTheirNgosQuickReports()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithStaff("Sam"))
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var quickReportId = scenarioData.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);

        // Act
        scenarioData.NgoByName(ScenarioNgos.Alfa).StaffByName("Sam")
            .PutWithoutResponse(
                $"/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}:status", new
                {
                    FollowUpStatus = QuickReportFollowUpStatus.NeedsFollowUp.ToString()
                });

        // Assert
        var quickReport = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<QuickReportDetailedModel>(
                $"/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}");

        quickReport.FollowUpStatus.Should().Be(QuickReportFollowUpStatus.NeedsFollowUp);
    }
}
