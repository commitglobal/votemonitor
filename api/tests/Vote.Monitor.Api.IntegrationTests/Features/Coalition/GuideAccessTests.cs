using System.Net;
using System.Net.Http.Json;
using Feature.ObserverGuide.Model;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using ListGuidesResponse = Feature.ObserverGuide.List.Response;

namespace Vote.Monitor.Api.IntegrationTests.Features.Coalition;

using static ApiTesting;

public class GuideAccessTests : BaseApiTestFixture
{
    [Test]
    public void ShouldGrantAccessToObservers_WhenNotInACoalition()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er
                    .WithMonitoringNgo(ScenarioNgos.Alfa,
                        alfa => alfa.WithMonitoringObserver(ScenarioObserver.Alice).WithGuide()))
            .Please();

        // Act
        var aliceGuides = scenarioData
            .ObserverByName(ScenarioObserver.Alice)
            .GetResponse<ListGuidesResponse>($"/api/election-rounds/{scenarioData.ElectionRoundId}/observer-guide");

        // Assert
        aliceGuides.Guides.Should().NotBeEmpty();
    }

    [Test]
    public void ShouldNotGrantGuideAccessForMonitoringObservers_WhenCreatingNewGuide()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er
                    .WithMonitoringNgo(ScenarioNgos.Alfa, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Alice))
                    .WithMonitoringNgo(ScenarioNgos.Beta, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Bob))
                    .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta]))
            .Please();

        // Act
        var electionRoundId = scenarioData.ElectionRoundId;
        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .CreateObserverGuide(electionRoundId);

        // Assert
        var aliceGuides = scenarioData
            .ObserverByName(ScenarioObserver.Alice)
            .GetResponse<ListGuidesResponse>($"/api/election-rounds/{electionRoundId}/observer-guide");

        var bobGuides = scenarioData
            .ObserverByName(ScenarioObserver.Bob)
            .GetResponse<ListGuidesResponse>($"/api/election-rounds/{electionRoundId}/observer-guide");

        aliceGuides.Guides.Should().BeEmpty();
        bobGuides.Guides.Should().BeEmpty();
    }

    [Test]
    public void ShouldNotGrantGuideAccessForMonitoringNgos_WhenCreatingNewGuide()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er
                    .WithMonitoringNgo(ScenarioNgos.Alfa, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Alice))
                    .WithMonitoringNgo(ScenarioNgos.Beta, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Bob))
                    .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta]))
            .Please();

        // Act
        var electionRoundId = scenarioData.ElectionRoundId;
        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .CreateObserverGuide(electionRoundId);

        // Assert
        var betaGuides = scenarioData
            .NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<ListGuidesResponse>($"/api/election-rounds/{electionRoundId}/observer-guide");

        betaGuides.Guides.Should().BeEmpty();
    }

    [Test]
    public void ShouldGrantGuideAccessForCoalitionMembersAndTheirObservers()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er
                    .WithMonitoringNgo(ScenarioNgos.Alfa,
                        alfa => alfa.WithMonitoringObserver(ScenarioObserver.Alice).WithGuide())
                    .WithMonitoringNgo(ScenarioNgos.Beta, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Bob))
                    .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta]))
            .Please();

        // Act
        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var guideId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).GuideId;

        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .PutWithoutResponse(
                $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/guides/{guideId}:access",
                new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Beta) } });

        // Assert
        var aliceGuides = scenarioData
            .ObserverByName(ScenarioObserver.Alice)
            .GetResponse<ListGuidesResponse>($"/api/election-rounds/{electionRoundId}/observer-guide");

        var bobGuides = scenarioData
            .ObserverByName(ScenarioObserver.Bob)
            .GetResponse<ListGuidesResponse>($"/api/election-rounds/{electionRoundId}/observer-guide");

        var betaGuides = scenarioData
            .NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<ListGuidesResponse>($"/api/election-rounds/{electionRoundId}/observer-guide");

        var guide = scenarioData
            .NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<ObserverGuideModel>($"/api/election-rounds/{electionRoundId}/observer-guide/{guideId}");

        aliceGuides.Guides.Should().BeEmpty();
        bobGuides.Guides.Select(x => x.Id).Should().HaveCount(1).And.BeEquivalentTo([guideId]);
        betaGuides.Guides.Select(x => x.Id).Should().HaveCount(1).And.BeEquivalentTo([guideId]);
        guide.Should().NotBeNull();
    }


    [Test]
    public void ShouldGrantGuideAccess_WhenGuideIsSharedWithOtherCoalitionMembers()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithNgo(ScenarioNgos.Delta)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er
                    .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta, ScenarioNgos.Delta],
                        c => c.WithGuide(sharedWithMembers: [ScenarioNgos.Beta])))
            .Please();

        // Act
        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var guideId = scenarioData.ElectionRound.Coalition.GuideId;

        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .PutWithoutResponse(
                $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/guides/{guideId}:access",
                new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Delta) } });

        // Assert
        var deltaGuides = scenarioData
            .NgoByName(ScenarioNgos.Delta).Admin
            .GetResponse<ListGuidesResponse>($"/api/election-rounds/{electionRoundId}/observer-guide");

        deltaGuides.Guides.Should().HaveCount(1);
    }

    [Test]
    public async Task ShouldNotUpdateGuideAccess_WhenCoalitionMember()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta],
                    cfg => cfg.WithGuide("A", [])))
            .Please();
        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var guideId = scenarioData.ElectionRound.Coalition.GuideId;

        var response = await scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .PutAsJsonAsync($"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/guides/{guideId}:access",
                new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Beta) } });

        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldNotUpdateGuideAccess_WhenCoalitionObserver()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithGuide("A", [])
                    .WithMonitoringObserver(ScenarioNgos.Alfa, ScenarioObserver.Alice)
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var guideId = scenarioData.ElectionRound.Coalition.GuideId;

        var response = await scenarioData.ObserverByName(ScenarioObserver.Alice)
            .PutAsJsonAsync($"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/guides/{guideId}:access",
                new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Beta) } });

        response.Should().HaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task ShouldNotUpdateGuideAccess_WhenUnauthorizedClients()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er.WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta],
                    c => c.WithGuide()))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var guideId = scenarioData.ElectionRound.Coalition.GuideId;

        var response = await CreateClient()
            .PutAsJsonAsync($"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/guides/{guideId}:access",
                new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Beta) } });

        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}
