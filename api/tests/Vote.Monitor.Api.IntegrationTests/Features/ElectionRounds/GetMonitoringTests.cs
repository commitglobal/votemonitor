using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using ElectionRoundsMonitoringResult = Vote.Monitor.Api.Feature.ElectionRound.Monitoring.Result;

namespace Vote.Monitor.Api.IntegrationTests.Features.ElectionRounds;

using static ApiTesting;

public class GetMonitoringTests : BaseApiTestFixture
{
    [Test]
    public void ShouldReturnCorrectElectionRoundDetails()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta])
            )
            .WithElectionRound(ScenarioElectionRound.B, er => er
                .WithMonitoringNgo(ScenarioNgo.Beta)
            )
            .WithElectionRound(ScenarioElectionRound.C, er => er
                .WithMonitoringNgo(ScenarioNgo.Alfa)
                .WithMonitoringNgo(ScenarioNgo.Beta)
            )
            .Please();

        var electionRoundAId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.A);
        var electionRoundBId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.B);
        var electionRoundCId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.C);
        // Act
        var alfaNgoElectionRounds = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<ElectionRoundsMonitoringResult>(
                $"/api/election-rounds:monitoring");

        var betaNgoElectionRounds = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<ElectionRoundsMonitoringResult>(
                $"/api/election-rounds:monitoring");

        // Assert
        alfaNgoElectionRounds.ElectionRounds
            .Should()
            .HaveCount(2);

        alfaNgoElectionRounds
            .ElectionRounds
            .First(x => x.ElectionRoundId == electionRoundAId)
            .IsCoalitionLeader
            .Should()
            .BeTrue();

        alfaNgoElectionRounds
            .ElectionRounds
            .First(x => x.ElectionRoundId == electionRoundCId)
            .IsCoalitionLeader
            .Should()
            .BeFalse();

        betaNgoElectionRounds.ElectionRounds
            .Should()
            .HaveCount(3);

        betaNgoElectionRounds
            .ElectionRounds
            .First(x => x.ElectionRoundId == electionRoundAId)
            .IsCoalitionLeader
            .Should()
            .BeFalse();

        betaNgoElectionRounds
            .ElectionRounds
            .First(x => x.ElectionRoundId == electionRoundBId)
            .IsCoalitionLeader
            .Should()
            .BeFalse();

        betaNgoElectionRounds
            .ElectionRounds
            .First(x => x.ElectionRoundId == electionRoundCId)
            .IsCoalitionLeader
            .Should()
            .BeFalse();
    }
}
