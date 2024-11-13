using System.Net;
using System.Net.Http.Json;
using Feature.Form.Submissions.ListEntries;
using Feature.Forms.Models;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.Features.Coalition;

using static ApiTesting;

public class DeleteTests : BaseApiTestFixture
{
    [Test]
    public async Task PlatformAdmin_ShouldDeleteCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithElectionRound(ElectionRounds.A, er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, []))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalition = scenarioData.ElectionRound.Coalition;

        var deleteResponse = await scenarioData.PlatformAdmin.DeleteAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalition.CoalitionId}");

        deleteResponse.Should().HaveStatusCode(HttpStatusCode.NoContent);

        var getCoalitionByIdResponse = await scenarioData.PlatformAdmin.GetAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalition.CoalitionId}");

        getCoalitionByIdResponse.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldRemoveDataForMonitoringNgos()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(Observers.Alice)
            .WithObserver(Observers.Bob)
            .WithNgo(Ngos.Alfa, ngo => ngo.WithAdmin(Ngos.Alfa.Anya))
            .WithNgo(Ngos.Beta, ngo => ngo.WithAdmin(Ngos.Beta.Dana))
            .WithElectionRound(ElectionRounds.A, er => er
                .WithPollingStation(PollingStations.Iasi)
                .WithPollingStation(PollingStations.Bacau)
                .WithPollingStation(PollingStations.Cluj)
                .WithMonitoringNgo(Ngos.Alfa, ngo => ngo.WithMonitoringObserver(Observers.Alice))
                .WithMonitoringNgo(Ngos.Beta, ngo => ngo.WithMonitoringObserver(Observers.Bob))
                .WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta],
                    cfg => cfg
                        .WithForm("Common", [Ngos.Beta],
                            form => form
                                .WithSubmission(Observers.Alice, PollingStations.Iasi)
                                .WithSubmission(Observers.Alice, PollingStations.Bacau)
                                .WithSubmission(Observers.Bob, PollingStations.Iasi)
                                .WithSubmission(Observers.Bob, PollingStations.Bacau))
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var deleteResponse = await scenarioData.PlatformAdmin.DeleteAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}");

        deleteResponse.Should().HaveStatusCode(HttpStatusCode.NoContent);
        var alfaNgoSubmissions = await scenarioData
            .NgoByName(Ngos.Alfa).Admin
            .GetResponseAsync<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry");

        var betaNgoSubmissions = await scenarioData
            .NgoByName(Ngos.Beta).Admin
            .GetResponseAsync<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry");

        var submission1 =
            scenarioData.ElectionRound.Coalition.Form.GetSubmissionId(Observers.Alice, PollingStations.Iasi);
        var submission2 =
            scenarioData.ElectionRound.Coalition.Form.GetSubmissionId(Observers.Alice, PollingStations.Bacau);

        alfaNgoSubmissions.Items.Select(x => x.SubmissionId)
            .Should()
            .HaveCount(2)
            .And
            .BeEquivalentTo([submission1, submission2]);

        betaNgoSubmissions.Items.Should().HaveCount(0);
    }

    [Test]
    public async Task ShouldKeepFormForLeader()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(Observers.Alice)
            .WithObserver(Observers.Bob)
            .WithNgo(Ngos.Alfa, ngo => ngo.WithAdmin(Ngos.Alfa.Anya))
            .WithNgo(Ngos.Beta, ngo => ngo.WithAdmin(Ngos.Beta.Dana))
            .WithElectionRound(ElectionRounds.A, er => er
                .WithPollingStation(PollingStations.Iasi)
                .WithPollingStation(PollingStations.Bacau)
                .WithPollingStation(PollingStations.Cluj)
                .WithMonitoringNgo(Ngos.Alfa, ngo => ngo.WithMonitoringObserver(Observers.Alice))
                .WithMonitoringNgo(Ngos.Beta, ngo => ngo.WithMonitoringObserver(Observers.Bob))
                .WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta],
                    cfg => cfg.WithForm("Common", [Ngos.Beta])
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var deleteResponse = await scenarioData.PlatformAdmin.DeleteAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}");

        deleteResponse.Should().HaveStatusCode(HttpStatusCode.NoContent);
        var formResult = await scenarioData
            .NgoByName(Ngos.Alfa).Admin
            .GetResponseAsync<PagedResponse<FormSlimModel>>(
                $"/api/election-rounds/{electionRoundId}/forms");

        formResult.Items.Should().HaveCount(1);
        formResult.Items.First().Id.Should().Be(scenarioData.ElectionRound.Coalition.Form.FormId);
    }

    [Test]
    public async Task ShouldRemoveFormAccessFromExCoalitionMembers()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(Observers.Alice)
            .WithObserver(Observers.Bob)
            .WithNgo(Ngos.Alfa, ngo => ngo.WithAdmin(Ngos.Alfa.Anya))
            .WithNgo(Ngos.Beta, ngo => ngo.WithAdmin(Ngos.Beta.Dana))
            .WithElectionRound(ElectionRounds.A, er => er
                .WithPollingStation(PollingStations.Iasi)
                .WithPollingStation(PollingStations.Bacau)
                .WithPollingStation(PollingStations.Cluj)
                .WithMonitoringNgo(Ngos.Alfa, ngo => ngo.WithMonitoringObserver(Observers.Alice))
                .WithMonitoringNgo(Ngos.Beta, ngo => ngo.WithMonitoringObserver(Observers.Bob))
                .WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta],
                    cfg => cfg
                        .WithForm("Common", [Ngos.Beta])
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var deleteResponse = await scenarioData.PlatformAdmin.DeleteAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}");

        deleteResponse.Should().HaveStatusCode(HttpStatusCode.NoContent);
        var formResult = await scenarioData
            .NgoByName(Ngos.Beta).Admin
            .GetResponseAsync<PagedResponse<FormSlimModel>>(
                $"/api/election-rounds/{electionRoundId}/forms");

        formResult.Items.Should().HaveCount(0);
    }

    [Test]
    public async Task NgoAdmin_CannotUpdateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa, ngo => ngo.WithAdmin())
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ElectionRounds.A, er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, []))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var coalitionResponseMessage = await scenarioData.NgoByName(Ngos.Alfa).Admin.PutAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Alfa).NgoId,
                NgoMembersIds = new[] { scenarioData.NgoByName(Ngos.Beta) }
            });

        coalitionResponseMessage.Should().HaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task Observer_CannotUpdateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(Observers.Alice)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ElectionRounds.A, er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta]))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var coalitionResponseMessage = await scenarioData.Observer.PutAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Alfa).NgoId,
                NgoMembersIds = Array.Empty<object>()
            });

        coalitionResponseMessage.Should().HaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task UnauthorizedClients_CannotUpdateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ElectionRounds.A, er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta]))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var coalitionResponseMessage = await CreateClient().PutAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Alfa).NgoId,
                NgoMembersIds = Array.Empty<object>()
            });

        coalitionResponseMessage.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}
