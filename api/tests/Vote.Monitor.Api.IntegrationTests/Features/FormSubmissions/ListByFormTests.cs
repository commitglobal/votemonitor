using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Api.IntegrationTests.TestCases;
using Vote.Monitor.Core.Models;
using ListByFormResponse = Feature.Form.Submissions.ListByForm.Response;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

public class ListByFormTests : BaseApiTestFixture
{
    [Test]
    public void ShouldExcludeCoalitionMembersForms_WhenCoalitionLeader_And_DataSourceNgo()
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
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))
                    .WithForm("Beta only", [ScenarioNgos.Beta],
                        betaForm => betaForm.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        // Act
        var forms = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<ListByFormResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byForm?dataSource=Ngo");

        // Assert
        forms
            .AggregatedForms
            .Should()
            .HaveCount(3);

        var aFormData = forms.AggregatedForms.First(x => x.FormCode == "A");
        aFormData.NumberOfSubmissions.Should().Be(1);

        var psiFormData = forms.AggregatedForms.First(x => x.FormCode == "PSI");
        psiFormData.NumberOfSubmissions.Should().Be(0);

        var commonFormData = forms.AggregatedForms.First(x => x.FormCode == "Common");
        commonFormData.NumberOfSubmissions.Should().Be(1);
    }

    [Test]
    public void ShouldIncludeCoalitionMembersResponses_WhenCoalitionLeader_And_DataSourceCoalition()
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
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))
                    .WithForm("Beta only", [ScenarioNgos.Beta],
                        betaForm => betaForm.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        // Act
        var forms = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<ListByFormResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byForm?dataSource=Coalition");

        // Assert
        forms
            .AggregatedForms
            .Should()
            .HaveCount(4);

        // A form is not visible since it was not shared with anyone
        var psiFormData = forms.AggregatedForms.First(x => x.FormCode == "PSI");
        psiFormData.NumberOfSubmissions.Should().Be(0);

        var commonFormData = forms.AggregatedForms.First(x => x.FormCode == "Common");
        commonFormData.NumberOfSubmissions.Should().Be(2);

        var betaFormData = forms.AggregatedForms.First(x => x.FormCode == "Beta only");
        betaFormData.NumberOfSubmissions.Should().Be(1);
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
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob)
                    .WithForm("B",
                        form => form.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("A", [ScenarioNgos.Alfa],
                        form => form.WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi))
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta],
                        commonForm => commonForm
                            .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))
                    .WithForm("Beta only", [ScenarioNgos.Beta],
                        betaForm => betaForm.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        // Act
        var forms = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<ListByFormResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byForm?dataSource={dataSource}");

        // Assert
        forms
            .AggregatedForms
            .Should()
            .HaveCount(4);

        // A form is not visible since it was not shared with anyone
        forms.AggregatedForms.Should().HaveCount(4);
        var psiFormData = forms.AggregatedForms.First(x => x.FormCode == "PSI");
        psiFormData.NumberOfSubmissions.Should().Be(0);

        var commonFormData = forms.AggregatedForms.First(x => x.FormCode == "Common");
        commonFormData.NumberOfSubmissions.Should().Be(1);

        var betaFormData = forms.AggregatedForms.First(x => x.FormCode == "Beta only");
        betaFormData.NumberOfSubmissions.Should().Be(1);

        var bFormData = forms.AggregatedForms.First(x => x.FormCode == "B");
        bFormData.NumberOfSubmissions.Should().Be(1);
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

        // Act
        var response = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<ListByFormResponse>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byForm?dataSource=Coalition&coalitionMemberId={betaNgoId}");
        // Assert

        // A form is not visible since it was not shared with anyone
        response.AggregatedForms.Should().HaveCount(2);

        var commonFormData = response.AggregatedForms.First(x => x.FormCode == "Common");
        commonFormData.NumberOfSubmissions.Should().Be(3);

        var betaFormData = response.AggregatedForms.First(x => x.FormCode == "Beta only");
        betaFormData.NumberOfSubmissions.Should().Be(1);
    }
}
