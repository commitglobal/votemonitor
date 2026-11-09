using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.SavedViews;

using static ApiTesting;

public class SavedViewsTests : BaseApiTestFixture
{
    private const string Sam = "Sam";
    private const string Tom = "Tom";
    private const string Ada = "Ada";

    public record CreatorResponse(Guid Id, string DisplayName);

    public record ViewResponse(
        Guid Id,
        string RecordType,
        string Name,
        string? Description,
        string Access,
        JsonElement State,
        CreatorResponse CreatedBy,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        bool CanEdit,
        bool CanChangeAccess);

    public record ViewsResponse(ViewResponse[] Views);

    /// <summary>Alfa has two admins and two staff members; Beta is another monitoring NGO in the round.</summary>
    private ScenarioData Scenario() =>
        ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithAdmin(Ada).WithStaff(Sam).WithStaff(Tom))
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithMonitoringNgo(ScenarioNgos.Beta))
            .Please();

    private static string ViewsUri(Guid electionRoundId) => $"/api/election-rounds/{electionRoundId}/views";

    private static object State(string view = "board") => new
    {
        filter = new[] { new { field = "severity", op = "in", values = new[] { "Urgent" } } },
        view,
        groupBy = "stage"
    };

    private static HttpResponseMessage Create(HttpClient client, Guid electionRoundId, string name,
        string access = "Ngo", string recordType = "QuickReport", object? state = null, string? description = null) =>
        client.PostAsJsonAsync(ViewsUri(electionRoundId),
                new { recordType, name, description, access, state = state ?? State() })
            .GetAwaiter().GetResult();

    private static ViewResponse CreateView(HttpClient client, Guid electionRoundId, string name,
        string access = "Ngo", string recordType = "QuickReport")
    {
        var response = Create(client, electionRoundId, name, access, recordType);
        response.StatusCode.Should().Be(HttpStatusCode.Created, Body(response));
        return response.Content.ReadFromJsonAsync<ViewResponse>().GetAwaiter().GetResult()!;
    }

    private static HttpResponseMessage Update(HttpClient client, Guid electionRoundId, Guid viewId, string name,
        string access = "Ngo", object? state = null, string? description = null) =>
        client.PutAsJsonAsync($"{ViewsUri(electionRoundId)}/{viewId}",
                new { name, description, access, state = state ?? State() })
            .GetAwaiter().GetResult();

    private static HttpResponseMessage Delete(HttpClient client, Guid electionRoundId, Guid viewId) =>
        client.DeleteAsync($"{ViewsUri(electionRoundId)}/{viewId}").GetAwaiter().GetResult();

    private static ViewResponse[] List(HttpClient client, Guid electionRoundId, string recordType = "QuickReport") =>
        client.GetResponse<ViewsResponse>($"{ViewsUri(electionRoundId)}?recordType={recordType}").Views;

    private static string Body(HttpResponseMessage response) =>
        response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

    [Test]
    public void AdminCreatesListsUpdatesAndDeletesAView()
    {
        var data = Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;

        var response = Create(admin, er, "  Urgent, not reviewed  ", description: "   ");
        response.StatusCode.Should().Be(HttpStatusCode.Created, Body(response));
        Body(response).Should().Contain("\"description\":null");
        var view = response.Content.ReadFromJsonAsync<ViewResponse>().GetAwaiter().GetResult()!;

        view.Name.Should().Be("Urgent, not reviewed");
        view.Description.Should().BeNull();
        view.RecordType.Should().Be("QuickReport");
        view.Access.Should().Be("Ngo");
        view.State.GetProperty("view").GetString().Should().Be("board");
        view.State.GetProperty("groupBy").GetString().Should().Be("stage");
        view.State.GetProperty("filter")[0].GetProperty("values")[0].GetString().Should().Be("Urgent");
        view.CreatedBy.Id.Should().Be(data.NgoByName(ScenarioNgos.Alfa).AdminId);
        view.CreatedBy.DisplayName.Should().NotBeNullOrWhiteSpace();
        view.CanEdit.Should().BeTrue();
        view.CanChangeAccess.Should().BeTrue();

        List(admin, er).Should().ContainSingle().Which.Id.Should().Be(view.Id);
        List(admin, er, "IncidentReport").Should().BeEmpty();

        var updated = Update(admin, er, view.Id, "Urgent", "Private", State("list"), " Only urgent ones ");
        updated.StatusCode.Should().Be(HttpStatusCode.OK, Body(updated));
        var updatedView = updated.Content.ReadFromJsonAsync<ViewResponse>().GetAwaiter().GetResult()!;
        updatedView.Name.Should().Be("Urgent");
        updatedView.Description.Should().Be("Only urgent ones");
        updatedView.Access.Should().Be("Private");
        updatedView.State.GetProperty("view").GetString().Should().Be("list");
        updatedView.RecordType.Should().Be("QuickReport");
        updatedView.UpdatedAt.Should().BeOnOrAfter(updatedView.CreatedAt);

        Delete(admin, er, view.Id).StatusCode.Should().Be(HttpStatusCode.NoContent);
        List(admin, er).Should().BeEmpty();
        Delete(admin, er, view.Id).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public void StaffCreateUpdateAndDeleteTheirViews_EvenInAnArchivedRound()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;

        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:archive", new { });

        var view = CreateView(alfa.StaffByName(Sam), er, "Sam's view", recordType: "FormSubmission");
        view.CreatedBy.Id.Should().Be(alfa.StaffIdByName(Sam));
        view.CreatedBy.DisplayName.Should().Contain(Sam);

        List(alfa.StaffByName(Tom), er, "FormSubmission").Should().ContainSingle().Which.Id.Should().Be(view.Id);
        List(alfa.Admin, er, "FormSubmission").Should().ContainSingle();

        Update(alfa.StaffByName(Sam), er, view.Id, "Renamed").StatusCode.Should().Be(HttpStatusCode.OK);
        Delete(alfa.StaffByName(Sam), er, view.Id).StatusCode.Should().Be(HttpStatusCode.NoContent);
        List(alfa.Admin, er, "FormSubmission").Should().BeEmpty();
    }

    [Test]
    public void PrivateViewsAreHiddenFromOtherMembers()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;

        var view = CreateView(alfa.StaffByName(Sam), er, "Mine", "Private");

        List(alfa.StaffByName(Sam), er).Should().ContainSingle().Which.Id.Should().Be(view.Id);
        List(alfa.StaffByName(Tom), er).Should().BeEmpty();
        List(alfa.Admin, er).Should().BeEmpty();

        Update(alfa.StaffByName(Tom), er, view.Id, "Theirs", "Private").StatusCode.Should().Be(HttpStatusCode.NotFound);
        Update(alfa.Admin, er, view.Id, "Theirs", "Private").StatusCode.Should().Be(HttpStatusCode.NotFound);
        Delete(alfa.StaffByName(Tom), er, view.Id).StatusCode.Should().Be(HttpStatusCode.NotFound);
        Delete(alfa.Admin, er, view.Id).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public void SharedViewsCanBeChangedByTheirCreatorAndNgoAdminsOnly()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;

        var view = CreateView(alfa.StaffByName(Sam), er, "Shared");

        var asTom = List(alfa.StaffByName(Tom), er).Single();
        asTom.CanEdit.Should().BeFalse();
        asTom.CanChangeAccess.Should().BeFalse();
        var asAdmin = List(alfa.Admin, er).Single();
        asAdmin.CanEdit.Should().BeTrue();
        asAdmin.CanChangeAccess.Should().BeFalse();

        Update(alfa.StaffByName(Tom), er, view.Id, "Tom's").StatusCode.Should().Be(HttpStatusCode.Forbidden);
        Delete(alfa.StaffByName(Tom), er, view.Id).StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var byAdmin = Update(alfa.Admin, er, view.Id, "Renamed by an admin");
        byAdmin.StatusCode.Should().Be(HttpStatusCode.OK, Body(byAdmin));
        Update(alfa.StaffByName(Sam), er, view.Id, "Renamed by Sam").StatusCode.Should().Be(HttpStatusCode.OK);

        Delete(alfa.AdminByName(Ada), er, view.Id).StatusCode.Should().Be(HttpStatusCode.NoContent);
        List(alfa.StaffByName(Sam), er).Should().BeEmpty();
    }

    [Test]
    public void OnlyTheCreatorChangesAViewsAccess()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;

        var view = CreateView(alfa.StaffByName(Sam), er, "Shared");

        Update(alfa.Admin, er, view.Id, "Shared", "Private").StatusCode.Should().Be(HttpStatusCode.Forbidden);
        List(alfa.Admin, er).Single().Access.Should().Be("Ngo");

        Update(alfa.StaffByName(Sam), er, view.Id, "Shared", "Private").StatusCode.Should().Be(HttpStatusCode.OK);
        List(alfa.Admin, er).Should().BeEmpty();
        List(alfa.StaffByName(Sam), er).Single().Access.Should().Be("Private");

        Update(alfa.StaffByName(Sam), er, view.Id, "Shared", "Ngo").StatusCode.Should().Be(HttpStatusCode.OK);
        List(alfa.Admin, er).Should().ContainSingle();
    }

    [Test]
    public void AnotherNgosViewsAreNotFound()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;

        var view = CreateView(alfa.Admin, er, "Alfa's");

        List(beta.Admin, er).Should().BeEmpty();
        Update(beta.Admin, er, view.Id, "Beta's").StatusCode.Should().Be(HttpStatusCode.NotFound);
        Delete(beta.Admin, er, view.Id).StatusCode.Should().Be(HttpStatusCode.NotFound);
        List(alfa.Admin, er).Should().ContainSingle();
    }

    [Test]
    public void PlatformAdminsAreForbidden()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var view = CreateView(data.NgoByName(ScenarioNgos.Alfa).Admin, er, "Alfa's");

        data.PlatformAdmin.GetAsync($"{ViewsUri(er)}?recordType=QuickReport").GetAwaiter().GetResult().StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        Create(data.PlatformAdmin, er, "Platform").StatusCode.Should().Be(HttpStatusCode.Forbidden);
        Update(data.PlatformAdmin, er, view.Id, "Platform").StatusCode.Should().Be(HttpStatusCode.Forbidden);
        Delete(data.PlatformAdmin, er, view.Id).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public void InvalidViewsAreRejected()
    {
        var data = Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;

        Create(admin, er, "").StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, "   ").StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, new string('a', 101)).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, "View", description: new string('a', 501)).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, "View", access: "Everyone").StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, "View", recordType: "Unknown").StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, "View", state: new[] { 1, 2 }).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, "View", state: "board").StatusCode.Should().Be(HttpStatusCode.BadRequest);
        admin.PostAsJsonAsync(ViewsUri(er), new { recordType = "QuickReport", name = "View", access = "Ngo" })
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Create(admin, er, "View", state: new { search = new string('a', 33 * 1024) }).StatusCode
            .Should().Be(HttpStatusCode.BadRequest);
        admin.GetAsync(ViewsUri(er)).GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var view = CreateView(admin, er, "View");
        Update(admin, er, view.Id, " ").StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Update(admin, er, view.Id, "View", state: new[] { "a" }).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Update(admin, er, view.Id, "View", state: new { search = new string('a', 33 * 1024) }).StatusCode
            .Should().Be(HttpStatusCode.BadRequest);

        List(admin, er).Should().ContainSingle().Which.Name.Should().Be("View");
    }

    [Test]
    public void ViewsAreListedSharedFirstThenPrivate_EachAToZ()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var sam = alfa.StaffByName(Sam);
        var er = data.ElectionRoundId;

        CreateView(sam, er, "zeta", "Private");
        CreateView(alfa.Admin, er, "beta");
        CreateView(sam, er, "Alpha", "Private");
        CreateView(sam, er, "Gamma");
        CreateView(alfa.Admin, er, "alpha");
        CreateView(alfa.Admin, er, "Admin's private", "Private");

        List(sam, er).Select(x => x.Name).Should().Equal("alpha", "beta", "Gamma", "Alpha", "zeta");
        List(alfa.Admin, er).Select(x => x.Name).Should().Equal("alpha", "beta", "Gamma", "Admin's private");
    }

    [Test]
    public void CitizenReportViewsNeedTheCitizenReportingNgo()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;

        Create(alfa.Admin, er, "Citizen", recordType: "CitizenReport").StatusCode.Should().Be(HttpStatusCode.NotFound);
        List(alfa.Admin, er, "CitizenReport").Should().BeEmpty();

        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:enableCitizenReporting",
            new { ngoId = alfa.NgoId });

        var view = CreateView(alfa.StaffByName(Sam), er, "Citizen", recordType: "CitizenReport");
        List(alfa.Admin, er, "CitizenReport").Should().ContainSingle().Which.Id.Should().Be(view.Id);
        Create(data.NgoByName(ScenarioNgos.Beta).Admin, er, "Citizen", recordType: "CitizenReport").StatusCode
            .Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public void DeletingAMembersAccountRemovesTheirPrivateViewsOnly()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var sam = alfa.StaffByName(Sam);
        var er = data.ElectionRoundId;

        var shared = CreateView(sam, er, "Shared");
        CreateView(sam, er, "Private", "Private");
        var samName = shared.CreatedBy.DisplayName;

        data.PlatformAdmin.DeleteAsync($"/api/ngos/{alfa.NgoId}/staff/{alfa.StaffIdByName(Sam)}")
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.NoContent);

        var views = List(alfa.Admin, er);
        views.Should().ContainSingle();
        views[0].Id.Should().Be(shared.Id);
        views[0].CreatedBy.DisplayName.Should().Be(samName);
        views[0].CanEdit.Should().BeTrue();
        List(alfa.StaffByName(Tom), er).Single().CanEdit.Should().BeFalse();
    }
}
