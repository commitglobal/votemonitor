using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Dashboards;

using static ApiTesting;

public class DashboardsTests : BaseApiTestFixture
{
    private const string Sam = "Sam";

    public record CreatorResponse(Guid Id, string DisplayName);

    public record DashboardResponse(
        Guid Id,
        string Name,
        string? Description,
        JsonElement Charts,
        CreatorResponse CreatedBy,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        bool CanEdit);

    public record DashboardsResponse(DashboardResponse[] Dashboards);

    /// <summary>Alfa has an admin and a staff member; Beta is another monitoring NGO in the round.</summary>
    private ScenarioData Scenario() =>
        ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithStaff(Sam))
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithMonitoringNgo(ScenarioNgos.Beta))
            .Please();

    private static string Uri(Guid electionRoundId) => $"/api/election-rounds/{electionRoundId}/dashboards";

    private static object[] Charts(string kind = "observers") =>
    [
        new
        {
            id = "c1", title = "", kind, display = "chart",
            filter = new { filter = new[] { new { field = "hasFlaggedAnswers", op = "eq", value = true } } },
            layout = new { x = 0, y = 0, w = 3, h = 5 }
        }
    ];

    private static HttpResponseMessage Create(HttpClient client, Guid electionRoundId, string name, object? charts = null,
        string? description = null) =>
        client.PostAsJsonAsync(Uri(electionRoundId), new { name, description, charts = charts ?? Charts() })
            .GetAwaiter().GetResult();

    private static DashboardResponse CreateDashboard(HttpClient client, Guid electionRoundId, string name)
    {
        var response = Create(client, electionRoundId, name);
        response.StatusCode.Should().Be(HttpStatusCode.Created, Body(response));
        return response.Content.ReadFromJsonAsync<DashboardResponse>().GetAwaiter().GetResult()!;
    }

    private static HttpResponseMessage Update(HttpClient client, Guid electionRoundId, Guid id, string name,
        object? charts = null) =>
        client.PutAsJsonAsync($"{Uri(electionRoundId)}/{id}", new { name, charts = charts ?? Charts() })
            .GetAwaiter().GetResult();

    private static HttpResponseMessage Get(HttpClient client, Guid electionRoundId, Guid id) =>
        client.GetAsync($"{Uri(electionRoundId)}/{id}").GetAwaiter().GetResult();

    private static HttpResponseMessage Delete(HttpClient client, Guid electionRoundId, Guid id) =>
        client.DeleteAsync($"{Uri(electionRoundId)}/{id}").GetAwaiter().GetResult();

    private static DashboardResponse[] List(HttpClient client, Guid electionRoundId) =>
        client.GetResponse<DashboardsResponse>(Uri(electionRoundId)).Dashboards;

    private static string Body(HttpResponseMessage response) =>
        response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

    [Test]
    public void AdminCreatesListsUpdatesAndDeletesADashboard()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;

        var response = Create(alfa.Admin, er, "  Election day  ", description: "   ");
        response.StatusCode.Should().Be(HttpStatusCode.Created, Body(response));
        Body(response).Should().Contain("\"description\":null");
        var dashboard = response.Content.ReadFromJsonAsync<DashboardResponse>().GetAwaiter().GetResult()!;

        dashboard.Name.Should().Be("Election day");
        dashboard.Charts.GetArrayLength().Should().Be(1);
        dashboard.Charts[0].GetProperty("kind").GetString().Should().Be("observers");
        dashboard.Charts[0].GetProperty("layout").GetProperty("w").GetInt32().Should().Be(3);
        dashboard.CreatedBy.Id.Should().Be(alfa.AdminId);
        dashboard.CanEdit.Should().BeTrue();

        List(alfa.Admin, er).Should().ContainSingle().Which.Id.Should().Be(dashboard.Id);

        var updated = Update(alfa.Admin, er, dashboard.Id, "Election night", Charts("pollingStations"));
        updated.StatusCode.Should().Be(HttpStatusCode.OK, Body(updated));
        var after = updated.Content.ReadFromJsonAsync<DashboardResponse>().GetAwaiter().GetResult()!;
        after.Name.Should().Be("Election night");
        after.Charts[0].GetProperty("kind").GetString().Should().Be("pollingStations");
        after.UpdatedAt.Should().BeOnOrAfter(after.CreatedAt);

        Delete(alfa.Admin, er, dashboard.Id).StatusCode.Should().Be(HttpStatusCode.NoContent);
        List(alfa.Admin, er).Should().BeEmpty();
        Get(alfa.Admin, er, dashboard.Id).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public void StaffReadDashboardsButCannotChangeThem()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var dashboard = CreateDashboard(alfa.Admin, er, "Election day");
        var staff = alfa.StaffByName(Sam);

        List(staff, er).Should().ContainSingle().Which.CanEdit.Should().BeFalse();
        Get(staff, er, dashboard.Id).StatusCode.Should().Be(HttpStatusCode.OK);
        Create(staff, er, "Sam's").StatusCode.Should().Be(HttpStatusCode.Forbidden);
        Update(staff, er, dashboard.Id, "Sam's").StatusCode.Should().Be(HttpStatusCode.Forbidden);
        Delete(staff, er, dashboard.Id).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public void AdminsKeepWorkingOnDashboardsInAnArchivedRound()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:archive", new { });

        var dashboard = CreateDashboard(alfa.Admin, er, "After the election");
        Update(alfa.Admin, er, dashboard.Id, "Final").StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public void AnotherNgosDashboardsAreNotFound()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var dashboard = CreateDashboard(data.NgoByName(ScenarioNgos.Alfa).Admin, er, "Alfa's");
        var beta = data.NgoByName(ScenarioNgos.Beta).Admin;

        List(beta, er).Should().BeEmpty();
        Get(beta, er, dashboard.Id).StatusCode.Should().Be(HttpStatusCode.NotFound);
        Update(beta, er, dashboard.Id, "Beta's").StatusCode.Should().Be(HttpStatusCode.NotFound);
        Delete(beta, er, dashboard.Id).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public void RejectsMissingNamesChartsThatArentAnArrayAndTooManyCharts()
    {
        var data = Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;

        Create(admin, er, "  ").StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, new string('a', 101)).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, "Object", new { kind = "observers" }).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, "Many", Enumerable.Range(0, 51).Select(i => new { id = $"c{i}" }).ToArray())
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var empty = admin.PostAsJsonAsync(Uri(er), new { name = "Empty" }).GetAwaiter().GetResult();
        empty.StatusCode.Should().Be(HttpStatusCode.Created, Body(empty));
        empty.Content.ReadFromJsonAsync<DashboardResponse>().GetAwaiter().GetResult()!.Charts.GetArrayLength().Should().Be(0);
    }

    [Test]
    public void PlatformAdminsAreForbidden()
    {
        var data = Scenario();
        data.PlatformAdmin.GetAsync(Uri(data.ElectionRoundId)).GetAwaiter().GetResult().StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
    }
}
