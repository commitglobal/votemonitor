using System.Net;
using System.Net.Http.Json;
using Feature.Forms;
using Feature.Forms.Models;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Fakers;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.Features.Coalition;

using static ApiTesting;

public class FormAccessTests : BaseApiTestFixture
{
    [Test]
    public void ShouldNotGrantFormAccessForMonitoringObservers_WhenCreatingNewForm()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithObserver(Observers.Alice)
            .WithObserver(Observers.Bob)
            .WithElectionRound(ElectionRounds.A,
                er => er
                    .WithMonitoringNgo(Ngos.Alfa, alfa => alfa.WithMonitoringObserver(Observers.Alice))
                    .WithMonitoringNgo(Ngos.Beta, alfa => alfa.WithMonitoringObserver(Observers.Bob))
                    .WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta]))
            .Please();

        // Act
        var formRequest = Dummy.Form();
        var electionRoundId = scenarioData.ElectionRoundId;
        var ngoForm =
            scenarioData.NgoByName(Ngos.Alfa).Admin.PostWithResponse<CreateFormRequest>(
                $"/api/election-rounds/{electionRoundId}/forms",
                formRequest);

        scenarioData.NgoByName(Ngos.Alfa).Admin
            .PostAsync($"/api/election-rounds/{electionRoundId}/forms/{ngoForm.Id}:publish",
                null)
            .GetAwaiter().GetResult()
            .EnsureSuccessStatusCode();

        // Assert
        var aliceForms = scenarioData
            .ObserverByName(Observers.Alice)
            .GetResponse<NgoFormsResponseModel>($"/api/election-rounds/{electionRoundId}/forms:fetchAll");
        var bobForms = scenarioData
            .ObserverByName(Observers.Bob)
            .GetResponse<NgoFormsResponseModel>($"/api/election-rounds/{electionRoundId}/forms:fetchAll");
        
        aliceForms.Forms.Should().BeEmpty();
        bobForms.Forms.Should().BeEmpty();
    }
    
    [Test]
    public void ShouldNotGrantFormAccessForMonitoringNgos_WhenCreatingNewForm()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithObserver(Observers.Alice)
            .WithObserver(Observers.Bob)
            .WithElectionRound(ElectionRounds.A,
                er => er
                    .WithMonitoringNgo(Ngos.Alfa, alfa => alfa.WithMonitoringObserver(Observers.Alice))
                    .WithMonitoringNgo(Ngos.Beta, alfa => alfa.WithMonitoringObserver(Observers.Bob))
                    .WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta]))
            .Please();

        // Act
        var formRequest = Dummy.Form();
        var electionRoundId = scenarioData.ElectionRoundId;
        var ngoForm =
            scenarioData.NgoByName(Ngos.Alfa).Admin.PostWithResponse<CreateFormRequest>(
                $"/api/election-rounds/{electionRoundId}/forms",
                formRequest);

        scenarioData.NgoByName(Ngos.Alfa).Admin
            .PostAsync($"/api/election-rounds/{electionRoundId}/forms/{ngoForm.Id}:publish",
                null)
            .GetAwaiter().GetResult()
            .EnsureSuccessStatusCode();

        // Assert
        var betaForms = scenarioData
            .NgoByName(Ngos.Beta).Admin
            .GetResponse<PagedResponse<FormSlimModel>>($"/api/election-rounds/{electionRoundId}/forms");
        
        betaForms.Items.Should().BeEmpty();
    }

    [Test]
    public void ShouldGrantFormAccessForCoalitionMembersAndTheirObservers()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithObserver(Observers.Alice)
            .WithObserver(Observers.Bob)
            .WithElectionRound(ElectionRounds.A,
                er => er
                    .WithMonitoringNgo(Ngos.Alfa, alfa => alfa.WithMonitoringObserver(Observers.Alice).WithForm())
                    .WithMonitoringNgo(Ngos.Beta, alfa => alfa.WithMonitoringObserver(Observers.Bob))
                    .WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta]))
            .Please();

        // Act
        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.MonitoringNgoByName(Ngos.Alfa).FormId;

        scenarioData.NgoByName(Ngos.Alfa).Admin.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta) } });

        // Assert
        var aliceForms = scenarioData
            .ObserverByName(Observers.Alice)
            .GetResponse<NgoFormsResponseModel>($"/api/election-rounds/{electionRoundId}/forms:fetchAll");

        var bobForms = scenarioData
            .ObserverByName(Observers.Bob)
            .GetResponse<NgoFormsResponseModel>($"/api/election-rounds/{electionRoundId}/forms:fetchAll");

        var betaForms = scenarioData
            .NgoByName(Ngos.Beta).Admin
            .GetResponse<PagedResponse<FormSlimModel>>($"/api/election-rounds/{electionRoundId}/forms");
        
        var form = scenarioData
            .NgoByName(Ngos.Beta).Admin
            .GetResponse<FormFullModel>($"/api/election-rounds/{electionRoundId}/forms/{formId}");

        aliceForms.Forms.Should().BeEmpty();
        bobForms.Forms.Select(x=>x.Id).Should().HaveCount(1).And.BeEquivalentTo([formId]);
        betaForms.Items.Select(x=>x.Id).Should().HaveCount(1).And.BeEquivalentTo([formId]);
        form.Should().NotBeNull();
    }
    
    [Test]
    public async Task ShouldAllowMonitoringObserversToAddSubmissionsToCoalitionForms()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithObserver(Observers.Alice)
            .WithObserver(Observers.Bob)
            .WithElectionRound(ElectionRounds.A,
                er => er
                    .WithPollingStation(PollingStations.Iasi)
                    .WithPollingStation(PollingStations.Bacau)
                    .WithMonitoringNgo(Ngos.Alfa, alfa => alfa.WithMonitoringObserver(Observers.Alice).WithForm())
                    .WithMonitoringNgo(Ngos.Beta, alfa => alfa.WithMonitoringObserver(Observers.Bob))
                    .WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta]))
            .Please();

        // Act
        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.MonitoringNgoByName(Ngos.Alfa).FormId;

        scenarioData.NgoByName(Ngos.Alfa).Admin.PostWithoutResponse(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta) } });

        var pollingStationId = scenarioData.ElectionRound.PollingStationByName(PollingStations.Iasi);
        var questions  = scenarioData.ElectionRound.MonitoringNgoByName(Ngos.Alfa).Form.Questions;
        var submission = new FormSubmissionRequestFaker(formId, pollingStationId, questions).Generate();

        var observer = scenarioData.ObserverByName(Observers.Bob);

        var submissionId = await observer.PostAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            submission);

        // Assert
        submissionId.Should().HaveStatusCode(HttpStatusCode.OK);
    }
    
    
    [Test]
    public async Task ShouldNotUpdateFormAccess_WhenCoalitionMember()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ElectionRounds.A, er => er
                .WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta], cfg => cfg.WithForm("A", [])))
            .Please();
        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.Coalition.FormId;

        var response = await scenarioData.NgoByName(Ngos.Beta).Admin.PostAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta) } });

        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldNotUpdateFormAccess_WhenCoalitionObserver()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithObserver(Observers.Alice)
            .WithElectionRound(ElectionRounds.A, er => er
                .WithMonitoringNgo(Ngos.Alfa)
                .WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta], cfg => cfg
                    .WithForm("A", [])
                    .WithMonitoringObserver(Ngos.Alfa, Observers.Alice)
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.Coalition.FormId;

        var response = await scenarioData.ObserverByName(Observers.Alice).PostAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta) } });

        response.Should().HaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task ShouldNotUpdateFormAccess_WhenUnauthorizedClients()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(Ngos.Alfa)
            .WithNgo(Ngos.Beta)
            .WithElectionRound(ElectionRounds.A,
                er => er.WithCoalition(Coalitions.Youth, Ngos.Alfa, [Ngos.Beta], c => c.WithForm()))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.Coalition.FormId;

        var response = await CreateClient().PostAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(Ngos.Beta) } });

        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}
