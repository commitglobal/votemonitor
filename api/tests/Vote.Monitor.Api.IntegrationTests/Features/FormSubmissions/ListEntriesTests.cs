﻿using Feature.Form.Submissions.ListEntries;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Fakers;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Api.IntegrationTests.TestCases;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

public class ListEntriesTests : BaseApiTestFixture
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
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob)
                    .WithForm("B",
                        form => form.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("A", [ScenarioNgos.Alfa],
                        form => form.WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi))
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta],
                        commonForm => commonForm.WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                            .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))
                    .WithForm("Beta only", [ScenarioNgos.Beta],
                        betaForm => betaForm.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;

        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);
        
        // Act
        var alfaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource=Ngo");

        // Assert
        alfaNgoFormSubmissions.Items
            .Should()
            .HaveCount(2);

        alfaNgoFormSubmissions.Items.Select(x => x.MonitoringObserverId)
            .Distinct()
            .Should()
            .BeEquivalentTo([alice.MonitoringObserverId]);

        var iasiPollingStationId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var clujPollingStationId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        alfaNgoFormSubmissions.Items.Select(x => x.PollingStationId)
            .Should()
            .BeEquivalentTo([iasiPollingStationId, clujPollingStationId]);
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
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithMonitoringNgo(ScenarioNgos.Beta,
                    ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob)
                        .WithForm("A",
                            form => form.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta])
                    .WithForm("A", [ScenarioNgos.Alfa])
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                ))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.Coalition.FormByCode("A").Id;
        var coalitionFormId = scenarioData.ElectionRound.Coalition.FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.Coalition.FormByCode("A").Questions;
        var coalitionFormQuestions = scenarioData.ElectionRound.Coalition.Form.Questions;

        var iasiSubmission =
            new SubmissionFaker(coalitionFormId, psIasiId, coalitionFormQuestions).Generate();
        var clujSubmission = new SubmissionFaker(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new SubmissionFaker(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var alice = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice);

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);

        var firstSubmission = alice.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        var secondSubmission = alice.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        var thirdSubmission = bob.Client.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource=Coalition");

        // Assert
        alfaNgoFormSubmissions.Items
            .Select(x => x.SubmissionId)
            .Should()
            .HaveCount(3)
            .And.BeEquivalentTo([firstSubmission.Id, secondSubmission.Id, thirdSubmission.Id]);

        alfaNgoFormSubmissions.Items.Select(x => x.ObserverName).Should()
            .BeEquivalentTo(alice.DisplayName, alice.DisplayName, bob.MonitoringObserverId.ToString());
        alfaNgoFormSubmissions.Items.Select(x => x.Email).Should()
            .BeEquivalentTo(alice.Email, alice.Email, bob.MonitoringObserverId.ToString());
        alfaNgoFormSubmissions.Items.Select(x => x.PhoneNumber).Should()
            .BeEquivalentTo(alice.PhoneNumber, alice.PhoneNumber, bob.MonitoringObserverId.ToString());
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
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta])
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                ))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;
        var coalitionFormId = scenarioData.ElectionRound.Coalition.FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).Form.Questions;
        var coalitionFormQuestions = scenarioData.ElectionRound.Coalition.Form.Questions;
        var iasiSubmission =
            new SubmissionFaker(coalitionFormId, psIasiId, coalitionFormQuestions).Generate();
        var clujSubmission = new SubmissionFaker(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new SubmissionFaker(coalitionFormId, psBacauId, coalitionFormQuestions).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        var firstSubmission = alice.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        var secondSubmission = bob.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        var thirdSubmission = bob.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var betaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource={dataSource}");

        // Assert
        betaNgoFormSubmissions.Items
            .Select(x => x.SubmissionId)
            .Should()
            .HaveCount(2)
            .And.BeEquivalentTo([secondSubmission.Id, thirdSubmission.Id]);
    }

    [TestCaseSource(typeof(DataSourcesTestCases))]
    public void ShouldReturnNgoResponses_WhenGettingSubmissions_AsIndependentNgo(DataSource dataSource)
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
                    ngo => ngo.WithForm("A").WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta,
                    ngo => ngo.WithForm("B").WithMonitoringObserver(ScenarioObserver.Bob)))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;
        var betaFormId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Beta).FormId;

        var psIasiId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var psBacauId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        var psClujId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        var alfaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).Form.Questions;
        var betaFormQuestions = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Beta).Form.Questions;

        var iasiSubmission =
            new SubmissionFaker(alfaFormId, psIasiId, alfaFormQuestions).Generate();
        var clujSubmission = new SubmissionFaker(alfaFormId, psClujId, alfaFormQuestions).Generate();
        var bacauSubmission =
            new SubmissionFaker(betaFormId, psBacauId, betaFormQuestions).Generate();

        var alice = scenarioData.ObserverByName(ScenarioObserver.Alice);
        var bob = scenarioData.ObserverByName(ScenarioObserver.Bob);

        var firstSubmission = alice.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            clujSubmission);

        var secondSubmission = alice.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            iasiSubmission);

        var thirdSubmission = bob.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            bacauSubmission);

        // Act
        var alfaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource={dataSource}");

        var betaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource={dataSource}");

        // Assert
        alfaNgoFormSubmissions.Items
            .Select(x => x.SubmissionId)
            .Should()
            .HaveCount(2)
            .And.BeEquivalentTo([firstSubmission.Id, secondSubmission.Id]);

        betaNgoFormSubmissions.Items
            .Select(x => x.SubmissionId)
            .Should()
            .HaveCount(1)
            .And.BeEquivalentTo([thirdSubmission.Id]);
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

        var bob = scenarioData.ElectionRound
            .MonitoringNgoByName(ScenarioNgos.Beta)
            .ObserverByName(ScenarioObserver.Bob);
        
        // Act
        var alfaNgoFormSubmissions = scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .GetResponse<PagedResponse<FormSubmissionEntry>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource=Coalition&coalitionMemberId={betaNgoId}");
        
        // Assert 
        alfaNgoFormSubmissions.TotalCount.Should().Be(4);
        alfaNgoFormSubmissions.Items.Should().HaveCount(4);
        alfaNgoFormSubmissions.Items
            .Should()
            .AllSatisfy(submission => submission.MonitoringObserverId.Should().Be(bob.MonitoringObserverId));
    }
}
