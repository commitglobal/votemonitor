using System.Net;
using System.Net.Http.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Api.IntegrationTests.Features.IncidentReports;

using static ApiTesting;

public class GetByIdTests : BaseApiTestFixture
{
    public record ReviewStageResponse(string Name, bool IsDefault);

    public record IncidentReportResponse(Guid Id, string ObserverName, ReviewStageResponse? ReviewStage,
        object[]? Reviewers, string? Severity, string[]? Labels);

    [Test]
    public void TheObserverIsNamedByTheirFullName()
    {
        // Arrange: Alice (Alfa) sends an incident report.
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice)))
            .Please();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var alice = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).ObserverByName(ScenarioObserver.Alice);

        var incidentForm = Dummy.Form("INC");
        incidentForm.FormType = FormType.IncidentReporting;
        var form = admin.PostWithResponse<CreateFormRequest>($"/api/election-rounds/{er}/forms", incidentForm);
        admin.PostWithoutResponse($"/api/election-rounds/{er}/forms/{form.Id}:publish");

        var reportId = Guid.NewGuid();
        data.ObserverByName(ScenarioObserver.Alice)
            .PostAsJsonAsync($"/api/election-rounds/{er}/incident-reports",
                new { id = reportId, formId = form.Id, locationType = "NotRelatedToAPollingStation", locationDescription = "At the town hall" })
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        var report = admin.GetResponse<IncidentReportResponse>($"/api/election-rounds/{er}/incident-reports/{reportId}");

        // Assert: first and last name, as every list shows it (it once read "Smith Smith").
        report.ObserverName.Should().Be(alice.DisplayName);
        // The NGO's review stage, reviewers, severity and labels come with the report: a new one sits in the default
        // stage, with no reviewers, not assessed, unlabelled.
        report.ReviewStage.Should().NotBeNull();
        report.ReviewStage!.IsDefault.Should().BeTrue();
        report.Reviewers.Should().BeEmpty();
        report.Severity.Should().Be("NotAssessed");
        report.Labels.Should().BeEmpty();
    }
}
