using System.Net;
using System.Net.Http.Json;
using Feature.Form.Submissions.ListEntries;
using Feature.NgoCoalitions.Models;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;
using ListMonitoringNgosResponse = Feature.Monitoring.List.Response;

namespace Vote.Monitor.Api.IntegrationTests.Features.Coalition;

using static ApiTesting;

public class UpdateTests : BaseApiTestFixture
{
    [Test]
    public async Task PlatformAdmin_ShouldUpdateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithCoalition(ScenarioCoalition.Youth, Ngos.Alfa, []))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var newCoalitionName = Guid.NewGuid().ToString();
        var updatedCoalition = await scenarioData.PlatformAdmin.PutWithResponseAsync<CoalitionModel>(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new { CoalitionName = newCoalitionName, NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta) } });

        updatedCoalition.Should().NotBeNull();

        updatedCoalition.Name.Should().Be(newCoalitionName);
        updatedCoalition.Members.Should().HaveCount(2);

        updatedCoalition.LeaderId.Should().Be(scenarioData.NgoIdByName(Ngos.Alfa));
        updatedCoalition.Members.Select(x => x.Id).Should()
            .Contain([scenarioData.NgoIdByName(Ngos.Alfa), scenarioData.NgoIdByName(Ngos.Beta)]);
    }

    [Test]
    public async Task ShouldAddMembersAsMonitoringNgos_WhenTheyAreNotMonitoring()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithNgo(Ngos.Delta)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithCoalition(ScenarioCoalition.Youth, Ngos.Alfa, []))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        await scenarioData.PlatformAdmin.PutWithResponseAsync<CoalitionModel>(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta), scenarioData.NgoIdByName(Ngos.Delta) }
            });

        var monitoringNgos = await scenarioData.PlatformAdmin.GetResponseAsync<ListMonitoringNgosResponse>(
            $"/api/election-rounds/{electionRoundId}/monitoring-ngos");

        monitoringNgos.MonitoringNgos.Should().HaveCount(3);
        monitoringNgos.MonitoringNgos.Select(x => x.NgoId).Should()
            .Contain([
                scenarioData.NgoIdByName(Ngos.Alfa),
                scenarioData.NgoIdByName(Ngos.Beta),
                scenarioData.NgoIdByName(Ngos.Delta)
            ]);
    }

    [Test]
    public async Task NgosStayAsMonitoringNgos_WhenTheyAreKicked()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithNgo(Ngos.Delta)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithCoalition(ScenarioCoalition.Youth, Ngos.Alfa, []))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        await scenarioData.PlatformAdmin.PutWithResponseAsync<CoalitionModel>(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta), scenarioData.NgoIdByName(Ngos.Delta) }
            });


        var monitoringNgos = await scenarioData.PlatformAdmin.GetResponseAsync<ListMonitoringNgosResponse>(
            $"/api/election-rounds/{electionRoundId}/monitoring-ngos");
        monitoringNgos.MonitoringNgos.Should().HaveCount(3);
        monitoringNgos.MonitoringNgos.Select(x => x.NgoId).Should()
            .Contain([
                scenarioData.NgoIdByName(Ngos.Alfa),
                scenarioData.NgoIdByName(Ngos.Beta),
                scenarioData.NgoIdByName(Ngos.Delta)
            ]);
    }


    [Test]
    public async Task ShouldKeepObserverDatatForNgoForms_WhenNgoIsKicked()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithObserver(ScenarioObserver.Charlie)
            .WithObserver(ScenarioObserver.Dave)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(Ngos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioObserver.Bob)
                    .WithForm("A", form => form
                        .Publish()
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Bacau)
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Bacau)
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Cluj))
                )
                .WithMonitoringNgo(Ngos.Beta, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Charlie)
                    .WithMonitoringObserver(ScenarioObserver.Dave)
                    .WithForm("A", form => form
                        .Publish()
                        .WithSubmission(ScenarioObserver.Charlie, ScenarioPollingStation.Iasi)
                        .WithSubmission(ScenarioObserver.Charlie, ScenarioPollingStation.Bacau)
                        .WithSubmission(ScenarioObserver.Dave, ScenarioPollingStation.Bacau)
                        .WithSubmission(ScenarioObserver.Dave, ScenarioPollingStation.Cluj))
                )
                .WithCoalition(ScenarioCoalition.Youth, Ngos.Alfa, []))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        await scenarioData.PlatformAdmin.PutWithResponseAsync<CoalitionModel>(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta) }
            });

        var alfaNgoSubmissions = await scenarioData
            .NgoByName(Ngos.Alfa).Admin
            .GetResponseAsync<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry");

        var betaNgoSubmissions = await scenarioData
            .NgoByName(Ngos.Beta).Admin
            .GetResponseAsync<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry");

        alfaNgoSubmissions.Items.Should().HaveCount(4);
        betaNgoSubmissions.Items.Should().HaveCount(4);
    }

    [Test]
    public async Task ShouldRemoveDataForSharedFormsOfKickedNgos()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(Ngos.Alfa, ngo => ngo.WithAdmin(Ngos.Alfa.Anya))
            .WithNgo(Ngos.Beta, ngo => ngo.WithAdmin(Ngos.Beta.Dana))
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(Ngos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(Ngos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, Ngos.Alfa, [Ngos.Beta],
                    cfg => cfg
                        .WithForm("Common", [Ngos.Beta],
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


        await scenarioData.PlatformAdmin.PutWithResponseAsync<CoalitionModel>(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Alfa) }
            });

        var alfaNgoSubmissions = await scenarioData
            .NgoByName(Ngos.Alfa).Admin
            .GetResponseAsync<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry");

        var betaNgoSubmissions = await scenarioData
            .NgoByName(Ngos.Beta).Admin
            .GetResponseAsync<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry");

        var submission1 =
            scenarioData.ElectionRound.Coalition.FormData.GetSubmissionId(ScenarioObserver.Alice,
                ScenarioPollingStation.Iasi);
        var submission2 =
            scenarioData.ElectionRound.Coalition.FormData.GetSubmissionId(ScenarioObserver.Alice,
                ScenarioPollingStation.Bacau);

        alfaNgoSubmissions.Items
            .Should()
            .HaveCount(2)
            .And.Subject
            .Select(x => x.SubmissionId)
            .Should()
            .BeEquivalentTo([submission1, submission2]);

        betaNgoSubmissions.Items.Should().HaveCount(0);
    }


    [Test]
    public async Task NgoAdmin_CannotUpdateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithCoalition(ScenarioCoalition.Youth, Ngos.Alfa, []))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var coalitionResponseMessage = await scenarioData.Ngo.Admin.PutAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Alfa),
                NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta) }
            });

        coalitionResponseMessage.Should().HaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task Observer_CannotUpdateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithCoalition(ScenarioCoalition.Youth, Ngos.Alfa, []))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var coalitionResponseMessage = await scenarioData.Observer.PutAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Alfa),
                NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta) }
            });

        coalitionResponseMessage.Should().HaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task UnauthorizedClients_CannotUpdateCoalition()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithCoalition(ScenarioCoalition.Youth, Ngos.Alfa, []))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;

        var coalitionResponseMessage = await CreateClient().PutAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = scenarioData.NgoByName(Ngos.Alfa),
                NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta) }
            });

        coalitionResponseMessage.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}
