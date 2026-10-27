using System.Net;
using System.Net.Http.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Api.IntegrationTests.Features;

using static ApiTesting;

public class ArchivedReportFormsAggregatesTests : BaseApiTestFixture
{
    [TestCase("incident-reports", nameof(FormType.IncidentReporting))]
    [TestCase("citizen-reports", nameof(FormType.CitizenReporting))]
    public void ShouldReturnAggregatedSubmissions_WhenFormIsArchived(string reports, string formType)
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithMonitoringNgo(ScenarioNgos.Alfa))
            .Please();

        var electionRoundId = scenarioData.ElectionRoundId;
        var alfa = scenarioData.NgoByName(ScenarioNgos.Alfa);
        var admin = alfa.Admin;
        scenarioData.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{electionRoundId}:enableCitizenReporting",
            new { ngoId = alfa.NgoId });

        var formRequest = Dummy.Form(formType);
        formRequest.FormType = FormType.FromName(formType);
        var form = admin.PostWithResponse<CreateFormRequest>($"/api/election-rounds/{electionRoundId}/forms", formRequest);
        admin.PostWithoutResponse($"/api/election-rounds/{electionRoundId}/forms/{form.Id}:publish");
        admin.PostWithoutResponse($"/api/election-rounds/{electionRoundId}/forms/{form.Id}:obsolete");

        // Act
        var v1 = admin.GetAsync(
                $"/api/election-rounds/{electionRoundId}/{reports}/forms/{form.Id}:aggregated-submissions?dataSource=Ngo")
            .GetAwaiter().GetResult();
        var v2 = admin.PostAsJsonAsync(
                $"/api/election-rounds/{electionRoundId}/{reports}/forms/{form.Id}:aggregated-submissionsV2",
                new { DataSource = "Ngo" })
            .GetAwaiter().GetResult();

        // Assert
        v1.StatusCode.Should().Be(HttpStatusCode.OK);
        v2.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
