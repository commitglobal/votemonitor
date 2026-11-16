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

    public record HistoryEntry(string Kind, Guid? ActorId, string? ActorName);

    private static HistoryEntry[] History(HttpClient client, Guid electionRoundId, Guid reportId) =>
        client.GetResponse<HistoryEntry[]>($"/api/election-rounds/{electionRoundId}/reviews/IncidentReport/{reportId}/history");

    [Test]
    public void NgoMembersUnlockACompletedReport_SoItsObserverCanChangeItAgain_AndTheHistoryKeepsIt()
    {
        var (data, formId, reportId) = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;

        Save(data, formId, reportId).StatusCode.Should().Be(HttpStatusCode.Conflict, "a completed report is locked");
        Unlock(data.NgoByName(ScenarioNgos.Beta).Admin, er, reportId).Should().Be(HttpStatusCode.NotFound);
        Save(data, formId, reportId).StatusCode.Should().Be(HttpStatusCode.Conflict);

        // Staff unlock too.
        Unlock(alfa.StaffByName(Sam), er, reportId).Should().Be(HttpStatusCode.NoContent);
        Save(data, formId, reportId).StatusCode.Should().Be(HttpStatusCode.OK);
        History(alfa.Admin, er, reportId).Where(x => x.Kind == "UnlockedForEditing").Should().ContainSingle()
            .Which.ActorId.Should().Be(alfa.StaffIdByName(Sam));

        // Unlocking a report that isn't locked changes nothing and isn't kept.
        Unlock(alfa.Admin, er, reportId).Should().Be(HttpStatusCode.NoContent);
        History(alfa.Admin, er, reportId).Count(x => x.Kind == "UnlockedForEditing").Should().Be(1);

        // Locked again by its observer, an admin unlocks it again: a second entry.
        Save(data, formId, reportId, isCompleted: true).StatusCode.Should().Be(HttpStatusCode.OK);
        Unlock(alfa.Admin, er, reportId).Should().Be(HttpStatusCode.NoContent);
        History(alfa.Admin, er, reportId).Where(x => x.Kind == "UnlockedForEditing").Select(x => x.ActorId)
            .Should().BeEquivalentTo(new Guid?[] { alfa.AdminId, alfa.StaffIdByName(Sam) });
    }

    [Test]
    public void ReportsCannotBeUnlockedInAnArchivedRound()
    {
        var (data, _, reportId) = Scenario();
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{data.ElectionRoundId}:archive", new { });

        Unlock(data.NgoByName(ScenarioNgos.Alfa).Admin, data.ElectionRoundId, reportId).Should().Be(HttpStatusCode.BadRequest);
    }
}
