using System.Net;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

public class GetByIdTests : BaseApiTestFixture
{
    [Test]
    public void ShouldAnonymizedCoalitionMembersResponses_WhenCoalitionLeader()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta],
                        commonForm => commonForm
                            .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                    )
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var aliceSubmissionId = scenarioData.ElectionRound.Coalition.GetSubmissionId("Common", ScenarioObserver.Alice, ScenarioPollingStation.Cluj);
        var bobSubmissionId = scenarioData.ElectionRound.Coalition.GetSubmissionId("Common", ScenarioObserver.Bob, ScenarioPollingStation.Iasi);

        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        // Act
        var aliceSubmission = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<FormSubmissionView>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{aliceSubmissionId}");
        
        var bobSubmission = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<FormSubmissionView>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{bobSubmissionId}");

        // Assert
        aliceSubmission.Should().NotBeNull();
        bobSubmission.Should().NotBeNull();

        aliceSubmission.ObserverName.Should().Be(alice.DisplayName);
        aliceSubmission.Email.Should().Be(alice.Email);
        aliceSubmission.PhoneNumber.Should().Be(alice.PhoneNumber);
        aliceSubmission.MonitoringObserverId.Should().Be(alice.MonitoringObserverId);
        
        bobSubmission.ObserverName.Should().Be(bob.MonitoringObserverId.ToString());
        bobSubmission.Email.Should().Be(bob.MonitoringObserverId.ToString());
        bobSubmission.PhoneNumber.Should().Be(bob.MonitoringObserverId.ToString());
        bobSubmission.MonitoringObserverId.Should().Be(bob.MonitoringObserverId);
    }

    [Test]
    public void ShouldReturnNgoResponses_WhenCoalitionMember()
    {
          // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta],
                        commonForm => commonForm
                            .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                    )
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var bobSubmissionId = scenarioData.ElectionRound.Coalition.GetSubmissionId("Common", ScenarioObserver.Bob, ScenarioPollingStation.Iasi);

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        // Act
        var bobSubmission = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<FormSubmissionView>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{bobSubmissionId}");

        // Assert
        bobSubmission.Should().NotBeNull();

        bobSubmission.ObserverName.Should().Be(bob.DisplayName);
        bobSubmission.Email.Should().Be(bob.Email);
        bobSubmission.PhoneNumber.Should().Be(bob.PhoneNumber);
        bobSubmission.MonitoringObserverId.Should().Be(bob.MonitoringObserverId);
    }

    [Test]
    public async Task ShouldNotReturnResponses_WhenCoalitionMemberAccessesOtherMembersData()
    {
          // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta],
                        commonForm => commonForm
                            .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                    )
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var aliceSubmissionId = scenarioData.ElectionRound.Coalition.GetSubmissionId("Common", ScenarioObserver.Alice, ScenarioPollingStation.Cluj);

        // Act
        var aliceSubmissionResponse = await scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetAsync($"/api/election-rounds/{electionRoundId}/form-submissions/{aliceSubmissionId}");
        
        // Assert
        aliceSubmissionResponse.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
}
