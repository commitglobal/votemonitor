using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.MonitoringObservers;

using static ApiTesting;

/// <summary>The observers export holds the observers the list's filters match.</summary>
public class ExportObserversTests : BaseApiTestFixture
{
    [Test]
    public void Export_ShouldKeepOnlyObserversMatchingTheListFilters()
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
        var electionRoundId = scenarioData.ElectionRoundId;
        var alfa = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa);
        var alice = alfa.ObserverByName(ScenarioObserver.Alice);
        var bob = alfa.ObserverByName(ScenarioObserver.Bob);

        admin.DeleteAsync($"/api/election-rounds/{electionRoundId}/monitoring-observers/{bob.MonitoringObserverId}:suspend")
            .GetAwaiter().GetResult().EnsureSuccessStatusCode();

        var url = $"/api/election-rounds/{electionRoundId}/monitoring-observers:export";
        string[] Rows(string query) =>
            admin.GetStringAsync(url + query).GetAwaiter().GetResult()
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Skip(1)
                .ToArray();

        Rows("").Should().HaveCount(2);

        var suspended = Rows("?status=Suspended");
        suspended.Should().ContainSingle().Which.Should().StartWith(bob.MonitoringObserverId.ToString());

        Rows("?status=Active&status=Pending").Should().ContainSingle()
            .Which.Should().StartWith(alice.MonitoringObserverId.ToString());

        Rows($"?searchText={Uri.EscapeDataString(alice.Email)}").Should().ContainSingle()
            .Which.Should().StartWith(alice.MonitoringObserverId.ToString());
    }
}
