using Module.Answers.Aggregators;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Api.IntegrationTests.TestCases;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

class AggregatedFormData
{
    public Guid FormId { get; set; }
    public IReadOnlyList<Responder> Responders { get; set; }
    public int SubmissionCount { get; set; }
}

class GetAggregatedResponse
{
    public AggregatedFormData SubmissionsAggregate { get; set; }
}

public class GetAggregatedTests : BaseApiTestFixture
{
    [Test]
    public void ShouldExcludeCoalitionMembersResponses_WhenCoalitionLeader_And_DataSourceNgo()
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
                            .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Bacau)
                    )
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var commonFormId = scenarioData.ElectionRound.Coalition.Form.Id;

        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        // Act
        var aggregatedFormResponses = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<GetAggregatedResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{commonFormId}:aggregated?dataSource=Ngo");

        // Assert
        aggregatedFormResponses.SubmissionsAggregate.FormId
            .Should()
            .Be(commonFormId);

        aggregatedFormResponses.SubmissionsAggregate.Responders.Should().HaveCount(1);
        aggregatedFormResponses.SubmissionsAggregate.Responders.Should().BeEquivalentTo([
            new Responder(alice.MonitoringObserverId, alice.DisplayName, alice.Email, alice.PhoneNumber)
        ]);
        aggregatedFormResponses.SubmissionsAggregate.SubmissionCount.Should().Be(2);
    }

    [Test]
    public void ShouldAnonymizedCoalitionMembersResponses_WhenCoalitionLeader_And_DataSourceCoalition()
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
                            .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Bacau)
                    )
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var commonFormId = scenarioData.ElectionRound.Coalition.Form.Id;

        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        // Act
        var aggregatedFormResponses = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<GetAggregatedResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{commonFormId}:aggregated?dataSource=Coalition");

        // Assert
        aggregatedFormResponses.SubmissionsAggregate.FormId
            .Should()
            .Be(commonFormId);

        aggregatedFormResponses.SubmissionsAggregate.Responders.Should().HaveCount(2);
        aggregatedFormResponses.SubmissionsAggregate.Responders.Should().BeEquivalentTo([
            new Responder(alice.MonitoringObserverId, alice.DisplayName, alice.Email, alice.PhoneNumber),
            new Responder(bob.MonitoringObserverId, bob.MonitoringObserverId.ToString(),
                bob.MonitoringObserverId.ToString(), bob.MonitoringObserverId.ToString())
        ]);
        aggregatedFormResponses.SubmissionsAggregate.SubmissionCount.Should().Be(4);
    }

    [Test]
    public void ShouldAllowFilteringResponsesByNgoId_WhenCoalitionLeader_And_DataSourceCoalition()
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
                            .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Bacau)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Cluj)
                    )
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var commonFormId = scenarioData.ElectionRound.Coalition.Form.Id;

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        var betaNgoId = scenarioData.NgoIdByName(ScenarioNgos.Beta);

        // Act
        var aggregatedFormResponses = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<GetAggregatedResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{commonFormId}:aggregated?dataSource=Coalition&coalitionMemberId={betaNgoId}");

        // Assert
        aggregatedFormResponses.SubmissionsAggregate.FormId
            .Should()
            .Be(commonFormId);

        aggregatedFormResponses.SubmissionsAggregate.Responders.Should().HaveCount(1);
        aggregatedFormResponses.SubmissionsAggregate.Responders.Should().BeEquivalentTo([
            new Responder(bob.MonitoringObserverId, bob.MonitoringObserverId.ToString(),
                bob.MonitoringObserverId.ToString(), bob.MonitoringObserverId.ToString())
        ]);
        aggregatedFormResponses.SubmissionsAggregate.SubmissionCount.Should().Be(3);
    }

    [TestCaseSource(typeof(DataSourcesTestCases))]
    public void ShouldReturnNgoResponses_WhenCoalitionMember(DataSource dataSource)
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
                            .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Bacau)
                    )
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var commonFormId = scenarioData.ElectionRound.Coalition.Form.Id;

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        // Act
        var aggregatedFormResponses = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<GetAggregatedResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions/{commonFormId}:aggregated?dataSource={dataSource}");

        // Assert
        aggregatedFormResponses.SubmissionsAggregate.FormId
            .Should()
            .Be(commonFormId);

        aggregatedFormResponses.SubmissionsAggregate.Responders.Should().HaveCount(1);
        aggregatedFormResponses.SubmissionsAggregate.Responders.Should().BeEquivalentTo([
            new Responder(bob.MonitoringObserverId, bob.DisplayName, bob.Email, bob.PhoneNumber)
        ]);
        aggregatedFormResponses.SubmissionsAggregate.SubmissionCount.Should().Be(2);
    }
}
