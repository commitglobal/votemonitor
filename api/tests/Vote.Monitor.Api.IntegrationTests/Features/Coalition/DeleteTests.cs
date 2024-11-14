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
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, []))
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
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithAdmin(ScenarioNgos.Alfa.Anya))
            .WithNgo(ScenarioNgos.Beta, ngo => ngo.WithAdmin(ScenarioNgos.Beta.Dana))
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta],
                    cfg => cfg
                        .WithForm("Common", [ScenarioNgos.Beta],
                            form => form
                                .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                                .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Bacau)
                                .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                                .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Bacau))
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var deleteResponse = await scenarioData.PlatformAdmin.DeleteAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}");

        deleteResponse.Should().HaveStatusCode(HttpStatusCode.NoContent);
        var alfaNgoSubmissions = await scenarioData
            .NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponseAsync<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource=Coalition");

        var betaNgoSubmissions = await scenarioData
            .NgoByName(ScenarioNgos.Beta).Admin
            .GetResponseAsync<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource=Coalition");

        var submission1 =
            scenarioData.ElectionRound.Coalition.FormData.GetSubmissionId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var submission2 =
            scenarioData.ElectionRound.Coalition.FormData.GetSubmissionId(ScenarioObserver.Alice, ScenarioPollingStation.Bacau);

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
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithAdmin(ScenarioNgos.Alfa.Anya))
            .WithNgo(ScenarioNgos.Beta, ngo => ngo.WithAdmin(ScenarioNgos.Beta.Dana))
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta],
                    cfg => cfg.WithForm("Common", [ScenarioNgos.Beta])
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var deleteResponse = await scenarioData.PlatformAdmin.DeleteAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}");

        deleteResponse.Should().HaveStatusCode(HttpStatusCode.NoContent);
        var formResult = await scenarioData
            .NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponseAsync<PagedResponse<FormSlimModel>>(
                $"/api/election-rounds/{electionRoundId}/forms");

        formResult.Items.Should().HaveCount(1);
        formResult.Items.First().Id.Should().Be(scenarioData.ElectionRound.Coalition.FormId);
    }

    [Test]
    public async Task ShouldRemoveFormAccessFromExCoalitionMembers()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithAdmin(ScenarioNgos.Alfa.Anya))
            .WithNgo(ScenarioNgos.Beta, ngo => ngo.WithAdmin(ScenarioNgos.Beta.Dana))
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta],
                    cfg => cfg
                        .WithForm("Common", [ScenarioNgos.Beta])
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var deleteResponse = await scenarioData.PlatformAdmin.DeleteAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}");

        deleteResponse.Should().HaveStatusCode(HttpStatusCode.NoContent);
        var formResult = await scenarioData
            .NgoByName(ScenarioNgos.Beta).Admin
            .GetResponseAsync<PagedResponse<FormSlimModel>>(
                $"/api/election-rounds/{electionRoundId}/forms");

        formResult.Items.Should().HaveCount(0);
    }

    [Test]
    public async Task NgoAdmin_CannotUpdateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithAdmin())
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, []))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var coalitionResponseMessage = await scenarioData.NgoByName(ScenarioNgos.Alfa).Admin.PutAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(ScenarioNgos.Alfa).NgoId,
                NgoMembersIds = new[] { scenarioData.NgoByName(ScenarioNgos.Beta) }
            });

        coalitionResponseMessage.Should().HaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task Observer_CannotUpdateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta]))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var coalitionResponseMessage = await scenarioData.Observer.PutAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(ScenarioNgos.Alfa).NgoId,
                NgoMembersIds = Array.Empty<object>()
            });

        coalitionResponseMessage.Should().HaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task UnauthorizedClients_CannotUpdateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta]))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var coalitionResponseMessage = await CreateClient().PutAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(ScenarioNgos.Alfa).NgoId,
                NgoMembersIds = Array.Empty<object>()
            });

        coalitionResponseMessage.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}
