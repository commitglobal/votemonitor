using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.PollingStationInformation;

using static ApiTesting;

public class PsiFormTests : BaseApiTestFixture
{
    public record PsiFormResponse(Guid Id, int? NumberOfSubmissions);

    [Test]
    public void NgoUsersSeeHowManyPsiRecordsTheirObserversSent_OthersDont()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob)))
            .Please();
        var er = data.ElectionRoundId;
        var uri = $"/api/election-rounds/{er}/polling-station-information-form";
        data.PlatformAdmin.PostWithoutResponse(uri,
            new { defaultLanguage = "EN", languages = new[] { "EN" }, questions = Array.Empty<object>() });

        void SendPsi(ScenarioObserver observer, ScenarioPollingStation pollingStation) =>
            data.ObserverByName(observer).PostWithoutResponse(
                $"/api/election-rounds/{er}/polling-stations/{data.ElectionRound.PollingStationByName(pollingStation)}/information",
                new { answers = Array.Empty<object>() });

        SendPsi(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        SendPsi(ScenarioObserver.Alice, ScenarioPollingStation.Cluj);
        SendPsi(ScenarioObserver.Alice, ScenarioPollingStation.Cluj);
        SendPsi(ScenarioObserver.Bob, ScenarioPollingStation.Iasi);

        data.NgoByName(ScenarioNgos.Alfa).Admin.GetResponse<PsiFormResponse>(uri).NumberOfSubmissions.Should().Be(2);
        data.NgoByName(ScenarioNgos.Beta).Admin.GetResponse<PsiFormResponse>(uri).NumberOfSubmissions.Should().Be(1);
        data.PlatformAdmin.GetResponse<PsiFormResponse>(uri).NumberOfSubmissions.Should().BeNull();
        data.ObserverByName(ScenarioObserver.Alice).GetResponse<PsiFormResponse>(uri).NumberOfSubmissions.Should().BeNull();
    }
}
