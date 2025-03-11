using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

public class UpdateStatusTests : BaseApiTestFixture
{
    [Test]
    public void CoalitionAdminsCanUpdateStatusForFormSubmissionButNotForMembersObservers()
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


        // Act
        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .PutWithoutResponse(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{aliceSubmissionId}:status", new
                {
                    FollowUpStatus = SubmissionFollowUpStatus.NeedsFollowUp.ToString()
                });

        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .PutWithoutResponse(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{bobSubmissionId}:status", new
                {
                    FollowUpStatus = SubmissionFollowUpStatus.NeedsFollowUp.ToString()
                });
        
        // Assert
        var aliceSubmission = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<FormSubmissionView>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{aliceSubmissionId}");  
        
        var bobSubmission = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<FormSubmissionView>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{bobSubmissionId}");

        aliceSubmission.FollowUpStatus.Should().Be(SubmissionFollowUpStatus.NeedsFollowUp);
        bobSubmission.FollowUpStatus.Should().Be(SubmissionFollowUpStatus.NotApplicable);
    }

     [Test]
    public void CoalitionMemberAdminsCanUpdateStatusForFormSubmissionButNotForOtherMembersObservers()
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
        
        // Act
        scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .PutWithoutResponse(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{aliceSubmissionId}:status", new
                {
                    FollowUpStatus = SubmissionFollowUpStatus.NeedsFollowUp.ToString()
                });

        scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .PutWithoutResponse(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{bobSubmissionId}:status", new
                {
                    FollowUpStatus = SubmissionFollowUpStatus.NeedsFollowUp.ToString()
                });
        
        // Assert
        var aliceSubmission = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<FormSubmissionView>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{aliceSubmissionId}");  
        
        var bobSubmission = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<FormSubmissionView>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{bobSubmissionId}");

        aliceSubmission.FollowUpStatus.Should().Be(SubmissionFollowUpStatus.NotApplicable);
        bobSubmission.FollowUpStatus.Should().Be(SubmissionFollowUpStatus.NeedsFollowUp);
    }
}
