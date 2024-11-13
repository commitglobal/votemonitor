using System.Net;
using System.Net.Http.Json;
using Feature.Forms.Models;
using Feature.NgoCoalitions.Models;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;
using ListMonitoringNgosResponse = Feature.Monitoring.List.Response;

namespace Vote.Monitor.Api.IntegrationTests.Features.Coalition;

using static ApiTesting;

public class CreateTests : BaseApiTestFixture
{
    [Test]
    public async Task PlatformAdmin_ShouldCreateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ElectionRounds.A)
            .Please();

        var coalitionName = Guid.NewGuid().ToString();
        var electionRoundId = scenarioData.ElectionRoundId;

        var coalition = await scenarioData.PlatformAdmin.PostWithResponseAsync<CoalitionModel>(
            $"/api/election-rounds/{electionRoundId}/coalitions",
            new
            {
                CoalitionName = coalitionName,
                LeaderId = scenarioData.NgoIdByName(Ngos.Alfa),
                NgoMembersIds = new[] { scenarioData.NgoByName(Ngos.Beta).NgoId }
            });

        coalition.Should().NotBeNull();
        coalition.Name.Should().Be(coalitionName);
        coalition.Members.Should().HaveCount(2);

        coalition.LeaderId.Should().Be(scenarioData.NgoByName(Ngos.Alfa).NgoId);
        coalition.Members.Select(x => x.Id).Should()
            .Contain([scenarioData.NgoByName(Ngos.Alfa).NgoId, scenarioData.NgoByName(Ngos.Beta).NgoId]);
    }

    [Test]
    public async Task ShouldAddMembersAsMonitoringNgos_WhenTheyAreNotMonitoring()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithNgo(Ngos.Delta)
            .WithElectionRound(ElectionRounds.A,
                electionRound => electionRound.WithMonitoringNgo(Ngos.Alfa).WithMonitoringNgo(Ngos.Beta))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        await scenarioData.PlatformAdmin.PostWithResponseAsync<CoalitionModel>(
            $"/api/election-rounds/{electionRoundId}/coalitions",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Alfa).NgoId,
                NgoMembersIds = new[] { scenarioData.NgoByName(Ngos.Delta).NgoId }
            });

        var monitoringNgos = await scenarioData.PlatformAdmin.GetResponseAsync<ListMonitoringNgosResponse>(
            $"/api/election-rounds/{electionRoundId}/monitoring-ngos");

        monitoringNgos.MonitoringNgos.Should().HaveCount(3);
        monitoringNgos.MonitoringNgos.Select(x => x.NgoId).Should()
            .Contain([
                scenarioData.NgoByName(Ngos.Alfa).NgoId, scenarioData.NgoByName(Ngos.Beta).NgoId,
                scenarioData.NgoByName(Ngos.Delta).NgoId
            ]);
    }

    [Test]
    public async Task ShouldAddLeaderAsMonitoringNgos_WhenTheyAreNotMonitoring()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithNgo(Ngos.Delta)
            .WithElectionRound(ElectionRounds.A,
                electionRound => electionRound.WithMonitoringNgo(Ngos.Alfa).WithMonitoringNgo(Ngos.Beta))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        await scenarioData.PlatformAdmin.PostWithResponseAsync<CoalitionModel>(
            $"/api/election-rounds/{electionRoundId}/coalitions",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Delta).NgoId,
                NgoMembersIds = Array.Empty<Guid>()
            });

        var monitoringNgos = await scenarioData.PlatformAdmin.GetResponseAsync<ListMonitoringNgosResponse>(
            $"/api/election-rounds/{electionRoundId}/monitoring-ngos");

        monitoringNgos.MonitoringNgos.Should().HaveCount(3);
        monitoringNgos.MonitoringNgos.Select(x => x.NgoId).Should()
            .Contain([
                scenarioData.NgoByName(Ngos.Alfa).NgoId, scenarioData.NgoByName(Ngos.Beta).NgoId,
                scenarioData.NgoByName(Ngos.Delta).NgoId
            ]);
    }

    [Test]
    public async Task ShouldNotGiveAccessToNgoFormsForCoalitionMembers()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ElectionRounds.A,
                electionRound => electionRound.WithMonitoringNgo(Ngos.Alfa, ngo => ngo.WithForm("A")))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        await scenarioData.PlatformAdmin.PostWithResponseAsync<CoalitionModel>(
            $"/api/election-rounds/{electionRoundId}/coalitions",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Beta).NgoId,
                NgoMembersIds = Array.Empty<Guid>()
            });

        var formResult = await scenarioData
            .NgoByName(Ngos.Beta).Admin
            .GetResponseAsync<PagedResponse<FormSlimModel>>(
                $"/api/election-rounds/{electionRoundId}/forms");

        formResult.Items.Should().BeEmpty();
    }

    [Test]
    public async Task NgoAdmin_CannotCreateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithElectionRound(ElectionRounds.A)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        var coalitionResponseMessage = await scenarioData.NgoByName(Ngos.Alfa).Admin.PostAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Alfa).NgoId,
                NgoMembersIds = new[] { scenarioData.NgoByName(Ngos.Beta).NgoId }
            });

        coalitionResponseMessage.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task Observer_CannotCreateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(Observers.Alice)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ElectionRounds.A)
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        var coalitionResponseMessage = await scenarioData.Observer.PostAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Alfa).NgoId,
                NgoMembersIds = new[] { scenarioData.NgoByName(Ngos.Beta).NgoId }
            });

        coalitionResponseMessage.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task UnauthorizedClients_CannotCreateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ElectionRounds.A)
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        var coalitionResponseMessage = await CreateClient().PostAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Alfa).NgoId,
                NgoMembersIds = new[] { scenarioData.NgoByName(Ngos.Beta).NgoId }
            });

        coalitionResponseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
