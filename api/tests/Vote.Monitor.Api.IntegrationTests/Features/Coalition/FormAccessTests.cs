using System.Net;
using System.Net.Http.Json;
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
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er
                    .WithMonitoringNgo(ScenarioNgos.Alfa, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Alice))
                    .WithMonitoringNgo(ScenarioNgos.Beta, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Bob))
                    .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta]))
            .Please();

        // Act
        var formRequest = Dummy.Form();
        var electionRoundId = scenarioData.ElectionRoundId;
        var ngoForm =
            scenarioData.NgoByName(ScenarioNgos.Alfa).Admin.PostWithResponse<CreateFormRequest>(
                $"/api/election-rounds/{electionRoundId}/forms",
                formRequest);

        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .PostAsync($"/api/election-rounds/{electionRoundId}/forms/{ngoForm.Id}:publish",
                null)
            .GetAwaiter().GetResult()
            .EnsureSuccessStatusCode();

        // Assert
        var aliceForms = scenarioData
            .ObserverByName(ScenarioObserver.Alice)
            .GetResponse<NgoFormsResponseModel>($"/api/election-rounds/{electionRoundId}/forms:fetchAll");
        var bobForms = scenarioData
            .ObserverByName(ScenarioObserver.Bob)
            .GetResponse<NgoFormsResponseModel>($"/api/election-rounds/{electionRoundId}/forms:fetchAll");
        
        aliceForms.Forms.Should().BeEmpty();
        bobForms.Forms.Should().BeEmpty();
    }
    
    [Test]
    public void ShouldNotGrantFormAccessForMonitoringNgos_WhenCreatingNewForm()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er
                    .WithMonitoringNgo(ScenarioNgos.Alfa, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Alice))
                    .WithMonitoringNgo(ScenarioNgos.Beta, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Bob))
                    .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta]))
            .Please();

        // Act
        var formRequest = Dummy.Form();
        var electionRoundId = scenarioData.ElectionRoundId;
        var ngoForm =
            scenarioData.NgoByName(ScenarioNgos.Alfa).Admin.PostWithResponse<CreateFormRequest>(
                $"/api/election-rounds/{electionRoundId}/forms",
                formRequest);

        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .PostAsync($"/api/election-rounds/{electionRoundId}/forms/{ngoForm.Id}:publish",
                null)
            .GetAwaiter().GetResult()
            .EnsureSuccessStatusCode();

        // Assert
        var betaForms = scenarioData
            .NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<PagedResponse<FormSlimModel>>($"/api/election-rounds/{electionRoundId}/forms");
        
        betaForms.Items.Should().BeEmpty();
    }

    [Test]
    public void ShouldGrantFormAccessForCoalitionMembersAndTheirObservers()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er
                    .WithMonitoringNgo(ScenarioNgos.Alfa, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Alice).WithForm())
                    .WithMonitoringNgo(ScenarioNgos.Beta, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Bob))
                    .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta]))
            .Please();

        // Act
        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;

        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .PutWithoutResponse($"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Beta) } });

        // Assert
        var aliceForms = scenarioData
            .ObserverByName(ScenarioObserver.Alice)
            .GetResponse<NgoFormsResponseModel>($"/api/election-rounds/{electionRoundId}/forms:fetchAll");

        var bobForms = scenarioData
            .ObserverByName(ScenarioObserver.Bob)
            .GetResponse<NgoFormsResponseModel>($"/api/election-rounds/{electionRoundId}/forms:fetchAll");

        var betaForms = scenarioData
            .NgoByName(ScenarioNgos.Beta).Admin
            .GetResponse<PagedResponse<FormSlimModel>>($"/api/election-rounds/{electionRoundId}/forms");
        
        var form = scenarioData
            .NgoByName(ScenarioNgos.Beta).Admin
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
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er
                    .WithPollingStation(ScenarioPollingStation.Iasi)
                    .WithPollingStation(ScenarioPollingStation.Bacau)
                    .WithMonitoringNgo(ScenarioNgos.Alfa, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Alice).WithForm())
                    .WithMonitoringNgo(ScenarioNgos.Beta, alfa => alfa.WithMonitoringObserver(ScenarioObserver.Bob))
                    .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta]))
            .Please();

        // Act
        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;

        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .PutWithoutResponse($"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Beta) } });

        var pollingStationId = scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        var questions  = scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).Form.Questions;
        var submission = new FormSubmissionRequestFaker(formId, pollingStationId, questions).Generate();

        var observer = scenarioData.ObserverByName(ScenarioObserver.Bob);

        var submissionId = await observer.PostAsJsonAsync(
            $"/api/election-rounds/{electionRoundId}/form-submissions",
            submission);

        // Assert
        submissionId.Should().HaveStatusCode(HttpStatusCode.OK);
    }
    
    [Test]
    public void ShouldGrantFormAccess_WhenFormIsSharedWithOtherCoalitionMembers()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithNgo(ScenarioNgos.Delta)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er
                    .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta, ScenarioNgos.Delta], c=>c.WithForm(sharedWithMembers:[ScenarioNgos.Beta])))
            .Please();

        // Act
        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.Coalition.FormId;

        scenarioData.NgoByName(ScenarioNgos.Alfa).Admin
            .PutWithoutResponse($"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Delta) } });
        
        // Assert
        var deltaForms = scenarioData
            .NgoByName(ScenarioNgos.Delta).Admin
            .GetResponse<PagedResponse<FormSlimModel>>($"/api/election-rounds/{electionRoundId}/forms");

        deltaForms.Items.Should().HaveCount(1);
    }
    
    [Test]
    public async Task ShouldNotUpdateFormAccess_WhenCoalitionMember()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg.WithForm("A", [])))
            .Please();
        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.Coalition.FormId;

        var response = await scenarioData.NgoByName(ScenarioNgos.Beta).Admin
            .PutAsJsonAsync($"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Beta) } });

        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldNotUpdateFormAccess_WhenCoalitionObserver()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithObserver(ScenarioObserver.Alice)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("A", [])
                    .WithMonitoringObserver(ScenarioNgos.Alfa, ScenarioObserver.Alice)
                )
            )
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.Coalition.FormId;

        var response = await scenarioData.ObserverByName(ScenarioObserver.Alice)
            .PutAsJsonAsync($"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Beta) } });

        response.Should().HaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task ShouldNotUpdateFormAccess_WhenUnauthorizedClients()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A,
                er => er.WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], c => c.WithForm()))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var coalitionId = scenarioData.ElectionRound.CoalitionId;
        var formId = scenarioData.ElectionRound.Coalition.FormId;

        var response = await CreateClient()
            .PutAsJsonAsync($"/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}/forms/{formId}:access",
            new { NgoMembersIds = new[] { scenarioData.NgoIdByName(ScenarioNgos.Beta) } });

        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}
