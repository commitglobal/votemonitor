using System.Net;
using System.Net.Http.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Api.IntegrationTests.Features.IncidentReports;

using static ApiTesting;

public class UnlockIncidentReportTests : BaseApiTestFixture
{
    private const string Sam = "Sam";

    /// <summary>Alice (Alfa) completed an incident report; Beta is another monitoring NGO in the round.</summary>
    private (ScenarioData Data, Guid FormId, Guid ReportId) Scenario()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithStaff(Sam))
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta))
            .Please();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        var incidentForm = Dummy.Form("INC");
        incidentForm.FormType = FormType.IncidentReporting;
        var form = admin.PostWithResponse<CreateFormRequest>($"/api/election-rounds/{er}/forms", incidentForm);
        admin.PostWithoutResponse($"/api/election-rounds/{er}/forms/{form.Id}:publish");

        var reportId = Guid.NewGuid();
        Save(data, form.Id, reportId, isCompleted: true).StatusCode.Should().Be(HttpStatusCode.OK);
        return (data, form.Id, reportId);
    }

    private static HttpResponseMessage Save(ScenarioData data, Guid formId, Guid reportId, bool? isCompleted = null) =>
        data.ObserverByName(ScenarioObserver.Alice).PostAsJsonAsync(
            $"/api/election-rounds/{data.ElectionRoundId}/incident-reports",
            new
            {
                id = reportId,
                formId,
                locationType = "NotRelatedToAPollingStation",
                locationDescription = "At the town hall",
                isCompleted
            }).GetAwaiter().GetResult();

    private static HttpStatusCode Unlock(HttpClient client, Guid electionRoundId, Guid reportId) =>
        client.PutAsJsonAsync($"/api/election-rounds/{electionRoundId}/incident-reports/{reportId}:unlock", new { })
            .GetAwaiter().GetResult().StatusCode;

    [Test]
    public void AnNgoAdminUnlocksACompletedReport_SoItsObserverCanChangeItAgain()
    {
        var (data, formId, reportId) = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);

        Save(data, formId, reportId).StatusCode.Should().Be(HttpStatusCode.Conflict, "a completed report is locked");

        Unlock(alfa.StaffByName(Sam), data.ElectionRoundId, reportId).Should().Be(HttpStatusCode.Forbidden);
        Unlock(data.NgoByName(ScenarioNgos.Beta).Admin, data.ElectionRoundId, reportId).Should().Be(HttpStatusCode.NotFound);
        Save(data, formId, reportId).StatusCode.Should().Be(HttpStatusCode.Conflict);

        Unlock(alfa.Admin, data.ElectionRoundId, reportId).Should().Be(HttpStatusCode.NoContent);
        Save(data, formId, reportId).StatusCode.Should().Be(HttpStatusCode.OK);
        // Unlocking a report that isn't locked changes nothing.
        Unlock(alfa.Admin, data.ElectionRoundId, reportId).Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public void ReportsCannotBeUnlockedInAnArchivedRound()
    {
        var (data, _, reportId) = Scenario();
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{data.ElectionRoundId}:archive", new { });

        Unlock(data.NgoByName(ScenarioNgos.Alfa).Admin, data.ElectionRoundId, reportId).Should().Be(HttpStatusCode.BadRequest);
    }
}
