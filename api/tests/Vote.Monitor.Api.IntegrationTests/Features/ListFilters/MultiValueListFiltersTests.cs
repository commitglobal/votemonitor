using System.Net;
using System.Text.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Constants;

namespace Vote.Monitor.Api.IntegrationTests.Features.ListFilters;

using static ApiTesting;

/// <summary>List filters that take several values (a repeated query key) and keep rows matching any of them.</summary>
public class MultiValueListFiltersTests : BaseApiTestFixture
{
    [Test]
    public void PollingStationsList_ShouldKeepStationsMatchingAnyLevelValue()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi, level1: "Iasi")
                .WithPollingStation(ScenarioPollingStation.Bacau, level1: "Bacau")
                .WithPollingStation(ScenarioPollingStation.Cluj, level1: "Cluj"))
            .Please();

        var url = $"/api/election-rounds/{scenarioData.ElectionRoundId}/polling-stations:list";

        Count(scenarioData.PlatformAdmin, url + "?level1Filter=Iasi").Should().Be(1);
        Count(scenarioData.PlatformAdmin, url + "?level1Filter=Iasi&level1Filter=Cluj").Should().Be(2);
        Count(scenarioData.PlatformAdmin, url).Should().Be(3);
    }

    [Test]
    public void PollingStationsList_ShouldMatchPartOfTheNumber()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi, level1: "Iasi", number: "01/001")
                .WithPollingStation(ScenarioPollingStation.Bacau, level1: "Bacau", number: "01/002")
                .WithPollingStation(ScenarioPollingStation.Cluj, level1: "Cluj", number: "02/001"))
            .Please();

        var url = $"/api/election-rounds/{scenarioData.ElectionRoundId}/polling-stations:list";

        Count(scenarioData.PlatformAdmin, url + "?pollingStationNumberFilter=01%2F").Should().Be(2);
        Count(scenarioData.PlatformAdmin, url + "?pollingStationNumberFilter=001").Should().Be(2);
        Count(scenarioData.PlatformAdmin, url + "?pollingStationNumberFilter=02%2F001").Should().Be(1);
        Count(scenarioData.PlatformAdmin, url + "?pollingStationNumberFilter=001&level1Filter=Cluj").Should().Be(1);
    }

    [Test]
    public void LocationsList_ShouldKeepLocationsMatchingAnyLevelValue()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithElectionRound(ScenarioElectionRound.A)
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        scenarioData.PlatformAdmin.PostWithResponse<JsonElement>($"/api/election-rounds/{electionRoundId}/locations",
            new
            {
                Locations = new[] { "Iasi", "Bacau", "Cluj" }.Select((level1, index) => new
                {
                    Level1 = level1, Level2 = "Center", DisplayOrder = index, Tags = new Dictionary<string, string>()
                })
            });

        var url = $"/api/election-rounds/{electionRoundId}/locations:list";

        Count(scenarioData.PlatformAdmin, url + "?level1Filter=Iasi").Should().Be(1);
        Count(scenarioData.PlatformAdmin, url + "?level1Filter=Iasi&level1Filter=Cluj").Should().Be(2);
        Count(scenarioData.PlatformAdmin, url).Should().Be(3);
    }

    [Test]
    public void MonitoringObserversList_ShouldKeepObserversMatchingAnyStatus()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioObserver.Bob)))
            .Please();

        var admin = scenarioData.AdminOfNgo(ScenarioNgos.Alfa);
        var url = $"/api/election-rounds/{scenarioData.ElectionRoundId}/monitoring-observers";

        Count(admin, url).Should().Be(2);
        Count(admin, url + "?status=Active&status=Pending").Should().Be(2);
        Count(admin, url + "?status=Suspended").Should().Be(0);
    }

    [Test]
    public void PlatformLists_ShouldKeepRowsMatchingAnyValue()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A)
            .Please();

        var platformAdmin = scenarioData.PlatformAdmin;
        var ngoId = scenarioData.NgoIdByName(ScenarioNgos.Alfa);
        var unknownCountryId = Guid.NewGuid();

        Count(platformAdmin, "/api/observers?status=Active&status=Pending").Should().Be(1);
        Count(platformAdmin, "/api/observers?status=Deactivated").Should().Be(0);

        Count(platformAdmin, "/api/ngos?status=Activated&status=Deactivated").Should().Be(1);
        Count(platformAdmin, "/api/ngos?status=Deactivated").Should().Be(0);

        Count(platformAdmin, $"/api/ngos/{ngoId}/admins?status=Active&status=Pending").Should().Be(1);
        Count(platformAdmin, $"/api/ngos/{ngoId}/admins?status=Deactivated").Should().Be(0);
        Count(platformAdmin, $"/api/ngos/{ngoId}/staff?status=Active&status=Pending").Should().Be(0);

        Count(platformAdmin, "/api/election-rounds?electionRoundStatus=NotStarted&electionRoundStatus=Started")
            .Should().Be(1);
        Count(platformAdmin, "/api/election-rounds?electionRoundStatus=Archived").Should().Be(0);
        Count(platformAdmin, $"/api/election-rounds?countryId={CountriesList.RO.Id}&countryId={CountriesList.MD.Id}")
            .Should().Be(1);
        Count(platformAdmin, $"/api/election-rounds?countryId={CountriesList.MD.Id}").Should().Be(0);
        platformAdmin.GetAsync($"/api/election-rounds?countryId={CountriesList.RO.Id}&countryId={unknownCountryId}")
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.BadRequest);

        Count(platformAdmin, "/api/form-templates?formTemplateType=Opening&formTemplateType=Voting&formStatus=Drafted&formStatus=Published")
            .Should().Be(0);
    }

    [Test]
    public void PsiData_ShouldAcceptSeveralLevelAndNgoValues()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithMonitoringNgo(ScenarioNgos.Alfa))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var monitoringNgoId = scenarioData.ElectionRound.MonitoringNgoIdByName(ScenarioNgos.Alfa);
        var query = $"?level1Filter=Iasi&level1Filter=Cluj&level2Filter=Center&monitoringNgoId={monitoringNgoId}&monitoringNgoId={Guid.NewGuid()}";

        Count(scenarioData.PlatformAdmin, $"/api/election-rounds/{electionRoundId}/information:all{query}")
            .Should().Be(0);
        scenarioData.PlatformAdmin.GetAsync($"/api/election-rounds/{electionRoundId}/information:export{query}")
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static int Count(HttpClient client, string url) =>
        client.GetResponse<PagedResponse<JsonElement>>(url).TotalCount;
}
