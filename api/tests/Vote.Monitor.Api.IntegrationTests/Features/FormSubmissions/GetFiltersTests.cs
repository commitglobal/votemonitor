using NSubstitute;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Fakers;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using GetFiltersResponse = Feature.Form.Submissions.GetFilters.Response;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

public class GetFiltersTests : BaseApiTestFixture
{
    private readonly DateTime _now = DateTime.UtcNow.AddDays(1000);

    [Test]
    public void ShouldIncludeCoalitionMembersResponses_WhenGettingFiltersAsCoalitionLeader()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(Ngos.Alfa, ngo => ngo.WithForm("A", form => form.Publish()))
                .WithCoalition(ScenarioCoalition.Youth, Ngos.Alfa, [Ngos.Beta], cfg => cfg
                    .WithForm("Shared", [Ngos.Alfa, Ngos.Beta])
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                ))
            .Please();
        
        var firstSubmissionAt = _now.AddDays(-5);
        var secondSubmissionAt = _now.AddDays(-3);
        var thirdSubmissionAt = _now.AddDays(-1);
        
        ApiTimeProvider.UtcNow
            .Returns(firstSubmissionAt, secondSubmissionAt, thirdSubmissionAt);

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.MonitoringNgoByName(Ngos.Alfa).FormId;
        var coalitionFormId = scenarioData.ElectionRound.Coalition.FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(Ngos.Alfa).Form.Questions;
        var coalitionFormQuestions = scenarioData.ElectionRound.Coalition.Form.Questions;

        var iasiSubmission =
            new FormSubmissionRequestFaker(coalitionFormId, psIasiId, coalitionFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);
        
        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);
        
        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var filters = scenarioData.NgoByName(Ngos.Alfa).Admin
            .GetResponse<GetFiltersResponse>($"/api/election-rounds/{electionRoundId}/form-submissions:filters");

        // Assert
        filters.FormFilterOptions
            .Select(x => x.FormId)
            .Should()
            .HaveCount(2)
            .And
            .BeEquivalentTo([alfaFormId, coalitionFormId]);

        filters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should().BeCloseTo(firstSubmissionAt, TimeSpan.FromMicroseconds(100));
        filters.TimestampsFilterOptions.LastSubmissionTimestamp.Should().BeCloseTo(thirdSubmissionAt,TimeSpan.FromMicroseconds(100));
    }

    [Test]
    public void ShouldIncludeOnlyNgoResponses_WhenGettingFiltersAsCoalitionMember()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(Ngos.Alfa, ngo => ngo.WithForm("A", form => form.Publish()))
                .WithCoalition(ScenarioCoalition.Youth, Ngos.Alfa, [Ngos.Beta], cfg => cfg
                    .WithForm("Shared", [Ngos.Alfa, Ngos.Beta])
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                ))
            .Please();
        
        var firstSubmissionAt = _now.AddDays(-5);
        var secondSubmissionAt = _now.AddDays(-3);
        var thirdSubmissionAt = _now.AddDays(-1);
        
        ApiTimeProvider.UtcNow
            .Returns(firstSubmissionAt, secondSubmissionAt, thirdSubmissionAt);

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.MonitoringNgoByName(Ngos.Alfa).FormId;
        var coalitionFormId = scenarioData.ElectionRound.Coalition.FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(Ngos.Alfa).Form.Questions;
        var coalitionFormQuestions = scenarioData.ElectionRound.Coalition.Form.Questions;

        var iasiSubmission =
            new FormSubmissionRequestFaker(coalitionFormId, psIasiId, coalitionFormQuestions).Generate();
        var clujSubmission = new FormSubmissionRequestFaker(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new FormSubmissionRequestFaker(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);
        
        alice.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);
        
        bob.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var filters = scenarioData.NgoByName(Ngos.Beta).Admin
            .GetResponse<GetFiltersResponse>($"/api/election-rounds/{electionRoundId}/form-submissions:filters");

        // Assert
        filters.FormFilterOptions
            .Select(x => x.FormId)
            .Should()
            .HaveCount(1)
            .And
            .BeEquivalentTo([coalitionFormId]);

        filters.TimestampsFilterOptions.FirstSubmissionTimestamp.Should().BeCloseTo(secondSubmissionAt, TimeSpan.FromMicroseconds(100));
        filters.TimestampsFilterOptions.LastSubmissionTimestamp.Should().BeCloseTo(thirdSubmissionAt,TimeSpan.FromMicroseconds(100));
    }
}
