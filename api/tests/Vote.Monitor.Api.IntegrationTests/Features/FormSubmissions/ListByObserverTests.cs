using Feature.Form.Submissions.ListByObserver;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Api.IntegrationTests.TestCases;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

public class ListByObserverTests : BaseApiTestFixture
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
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta], form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Cluj)
                    )
                    .WithForm("A", [ScenarioNgos.Alfa],
                        form => form.WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Bacau))
                ))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        // Act
        var alfaNgoObservers = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<ObserverSubmissionOverview>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byObserver?dataSource=Ngo");

        // Assert
        alfaNgoObservers.Items
            .Select(x => x.MonitoringObserverId)
            .Should()
            .HaveCount(1)
            .And.BeEquivalentTo([alice.MonitoringObserverId]);

        alfaNgoObservers.Items.First().NumberOfFormsSubmitted.Should().Be(2);
        alfaNgoObservers.Items.First().IsOwnObserver.Should().BeTrue();
    }

    [Test]
    public void ShouldAnonymizeCoalitionMembersResponses_WhenCoalitionLeader_And_DataSourceCoalition()
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
                .WithMonitoringNgo(ScenarioNgos.Beta,
                    ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob)
                        .WithForm("B", form => form
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta], form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Bacau)
                    )
                    .WithForm("A", [ScenarioNgos.Alfa],
                        form => form.WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Bacau))
                ))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        // Act
        var coalitionObservers = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<ObserverSubmissionOverview>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byObserver?dataSource=Coalition");

        // Assert
        coalitionObservers.Items
            .Should()
            .HaveCount(2);

        var aliceData = coalitionObservers.Items
            .First(x => x.MonitoringObserverId == alice.MonitoringObserverId);

        var bobData = coalitionObservers.Items
            .First(x => x.MonitoringObserverId == bob.MonitoringObserverId);

        aliceData.NumberOfFormsSubmitted.Should().Be(2);
        bobData.NumberOfFormsSubmitted.Should().Be(1);

        aliceData.ObserverName.Should().Be(alice.DisplayName);
        bobData.ObserverName.Should().Be(bob.MonitoringObserverId.ToString());

        aliceData.Email.Should().Be(alice.Email);
        bobData.Email.Should().Be(bob.MonitoringObserverId.ToString());

        aliceData.PhoneNumber.Should().Be(alice.PhoneNumber);
        bobData.PhoneNumber.Should().Be(bob.MonitoringObserverId.ToString());
        aliceData.IsOwnObserver.Should().BeTrue();
        bobData.IsOwnObserver.Should().BeFalse();
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
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithForm("A"))
                .WithMonitoringNgo(ScenarioNgos.Beta,
                    ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob)
                        .WithForm("A", form => form
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta], form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Bacau))
                ))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        // Act
        var betaNgoObservers = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<PagedResponse<ObserverSubmissionOverview>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byObserver?dataSource={dataSource}");

        // Assert
        betaNgoObservers.Items
            .Should()
            .HaveCount(1);

        var bobData = betaNgoObservers.Items.First();

        bobData.NumberOfFormsSubmitted.Should().Be(2);
        bobData.ObserverName.Should().Be(bob.DisplayName);
        bobData.Email.Should().Be(bob.Email);
        bobData.PhoneNumber.Should().Be(bob.PhoneNumber);
        bobData.IsOwnObserver.Should().BeTrue();

    }

    [TestCaseSource(typeof(DataSourcesTestCases))]
    public void ShouldReturnNgoResponses_WhenIndependentNgo(DataSource dataSource)
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
                .WithMonitoringNgo(ScenarioNgos.Alfa,
                    ngo => ngo
                        .WithMonitoringObserver(ScenarioObserver.Alice)
                        .WithForm("A",
                            form => form.WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                                .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Bacau)))
                .WithMonitoringNgo(ScenarioNgos.Beta,
                    ngo => ngo
                        .WithMonitoringObserver(ScenarioObserver.Bob)
                        .WithForm("A",
                            form => form.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        // Act
        var alfaNgoObservers = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<ObserverSubmissionOverview>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byObserver?dataSource={dataSource}");

        var betaObservers = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<PagedResponse<ObserverSubmissionOverview>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byObserver?dataSource={dataSource}");

        // Assert
        var aliceData = alfaNgoObservers.Items
            .Should()
            .ContainSingle()
            .Subject;

        aliceData.NumberOfFormsSubmitted.Should().Be(2);
        aliceData.MonitoringObserverId.Should().Be(alice.MonitoringObserverId);
        aliceData.PhoneNumber.Should().Be(alice.PhoneNumber);
        aliceData.ObserverName.Should().Be(alice.DisplayName);
        aliceData.Email.Should().Be(alice.Email);
        aliceData.IsOwnObserver.Should().BeTrue();

        var bobData = betaObservers.Items
            .Should()
            .ContainSingle()
            .Subject;

        bobData.NumberOfFormsSubmitted.Should().Be(1);
        bobData.MonitoringObserverId.Should().Be(bob.MonitoringObserverId);
        bobData.PhoneNumber.Should().Be(bob.PhoneNumber);
        bobData.ObserverName.Should().Be(bob.DisplayName);
        bobData.Email.Should().Be(bob.Email);
        bobData.IsOwnObserver.Should().BeTrue();
    }

    [Test]
    public void ShouldAllowFilteringObserversByNgoId_WhenCoalitionLeader_And_DataSourceCoalition()
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
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob)
                    .WithForm("B",
                        form => form.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("A", [ScenarioNgos.Alfa],
                        form => form.WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi))
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta],
                        commonForm => commonForm
                            .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Bacau)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Cluj))
                    .WithForm("Beta only", [ScenarioNgos.Beta],
                        betaForm => betaForm.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var betaNgoId = scenarioData.NgoIdByName(ScenarioNgos.Beta);
        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);
        
        // Act
        var coalitionObservers = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<ObserverSubmissionOverview>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byObserver?dataSource=Coalition&coalitionMemberId={betaNgoId}");

        // Assert
        coalitionObservers.TotalCount.Should().Be(1);
        var bobData = coalitionObservers.Items
            .Should()
            .ContainSingle()
            .Subject;
        
        bobData.NumberOfFormsSubmitted.Should().Be(4);
        bobData.MonitoringObserverId.Should().Be(bob.MonitoringObserverId);
        bobData.PhoneNumber.Should().Be(bob.MonitoringObserverId.ToString());
        bobData.ObserverName.Should().Be(bob.MonitoringObserverId.ToString());
        bobData.Email.Should().Be(bob.MonitoringObserverId.ToString());
        bobData.IsOwnObserver.Should().BeFalse();
    }
}
