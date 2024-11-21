using System.Net;
using Feature.NgoCoalitions.Models;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Coalition;

using static ApiTesting;

public class GetMyTests : BaseApiTestFixture
{
    [Test]
    public void ShouldReturnCoalitionDetails_WhenIsPartOfCoalition()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta])
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaNgo = scenarioData.NgoByName(ScenarioNgos.Alfa);
        var betaNgo = scenarioData.NgoByName(ScenarioNgos.Beta);

        var alfaNgoAdmin = alfaNgo.Admin;
        var betaNgoAdmin = betaNgo.Admin;

        // Act
        var alfaNgoCoalition = alfaNgoAdmin
            .GetResponse<CoalitionModel>(
                $"/api/election-rounds/{electionRoundId}/coalitions:my");

        var betaNgoCoalition = betaNgoAdmin
            .GetResponse<CoalitionModel>(
                $"/api/election-rounds/{electionRoundId}/coalitions:my");

        // Assert
        alfaNgoCoalition.Should().BeEquivalentTo(betaNgoCoalition);
        alfaNgoCoalition.Members.Should().HaveCount(2);
        alfaNgoCoalition.Members.Select(x => x.Id).Should().BeEquivalentTo([alfaNgo.NgoId, betaNgo.NgoId]);
    }

    [Test]
    public async Task TaskShouldReturnNotFound_WhenNotInACoalition()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A)
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaNgoAdmin = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;

        // Act
        var alfaNgoCoalitionResponse = await alfaNgoAdmin
            .GetAsync(
                $"/api/election-rounds/{electionRoundId}/coalitions:my");

        // Assert
        alfaNgoCoalitionResponse.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
}
