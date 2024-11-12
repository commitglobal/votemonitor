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
            .WithElectionRound(ElectionRounds.A, er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, []))
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
            .WithElectionRound(ElectionRounds.A, er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, []))
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
            .WithElectionRound(ElectionRounds.A, er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, []))
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
            .WithObserver(Observers.Alice)
            .WithObserver(Observers.Bob)
            .WithObserver(Observers.Charlie)
            .WithObserver(Observers.Dave)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ElectionRounds.A, er => er
                .WithPollingStation(PollingStations.Iasi)
                .WithPollingStation(PollingStations.Bacau)
                .WithPollingStation(PollingStations.Cluj)
                .WithMonitoringNgo(Ngos.Alfa, ngo => ngo
                    .WithMonitoringObserver(Observers.Alice)
                    .WithMonitoringObserver(Observers.Bob)
                    .WithForm("A", form => form
                        .Publish()
                        .WithSubmission(Observers.Alice, PollingStations.Iasi)
                        .WithSubmission(Observers.Alice, PollingStations.Bacau)
                        .WithSubmission(Observers.Bob, PollingStations.Bacau)
                        .WithSubmission(Observers.Bob, PollingStations.Cluj))
                )
                .WithMonitoringNgo(Ngos.Beta, ngo => ngo
                    .WithMonitoringObserver(Observers.Charlie)
                    .WithMonitoringObserver(Observers.Dave)
                    .WithForm("A", form => form
                        .Publish()
                        .WithSubmission(Observers.Charlie, PollingStations.Iasi)
                        .WithSubmission(Observers.Charlie, PollingStations.Bacau)
                        .WithSubmission(Observers.Dave, PollingStations.Bacau)
                        .WithSubmission(Observers.Dave, PollingStations.Cluj))
                )
                .WithCoalition(Coalitions.Youth, Ngos.Alfa, []))
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
            scenarioData.ElectionRound.Coalition.Form.GetSubmissionId(Observers.Alice, PollingStations.Iasi);
        var submission2 =
            scenarioData.ElectionRound.Coalition.Form.GetSubmissionId(Observers.Alice, PollingStations.Bacau);
      
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
            .WithElectionRound(ElectionRounds.A, er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, []))
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
            .WithObserver(Observers.Alice)
            .WithElectionRound(ElectionRounds.A, er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, []))
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
            .WithElectionRound(ElectionRounds.A, er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, []))
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
