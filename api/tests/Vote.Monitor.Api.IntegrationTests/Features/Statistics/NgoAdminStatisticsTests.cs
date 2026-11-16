using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Api.IntegrationTests.Features.Statistics;

using static ApiTesting;

public class NgoAdminStatisticsTests : BaseApiTestFixture
{
    public record HistogramPoint(DateTime Bucket, int Value);

    public record StatisticsResponse(
        HistogramPoint[] QuickReportsHistogram,
        HistogramPoint[] IncidentReportsHistogram,
        HistogramPoint[] CitizenReportsHistogram);

    [Test]
    public void EachReportTypeCountsInItsOwnHistogram()
    {
        // Alice (Alfa) sent one quick report and one incident report; nobody sent a citizen report.
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi))
            .Please();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        var incidentForm = Dummy.Form("INC");
        incidentForm.FormType = FormType.IncidentReporting;
        var form = admin.PostWithResponse<CreateFormRequest>($"/api/election-rounds/{er}/forms", incidentForm);
        admin.PostWithoutResponse($"/api/election-rounds/{er}/forms/{form.Id}:publish");
        data.ObserverByName(ScenarioObserver.Alice).PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{er}/incident-reports",
            new
            {
                id = Guid.NewGuid(),
                formId = form.Id,
                locationType = "NotRelatedToAPollingStation",
                locationDescription = "At the town hall"
            });

        var statistics = admin.GetResponse<StatisticsResponse>($"/api/election-rounds/{er}/statistics?dataSource=Ngo");

        statistics.QuickReportsHistogram.Sum(x => x.Value).Should().Be(1);
        statistics.IncidentReportsHistogram.Sum(x => x.Value).Should().Be(1);
        statistics.CitizenReportsHistogram.Sum(x => x.Value).Should().Be(0);
    }
}
