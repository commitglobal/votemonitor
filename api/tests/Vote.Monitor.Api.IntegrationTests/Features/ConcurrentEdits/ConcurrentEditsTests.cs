using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.ConcurrentEdits;

using static ApiTesting;

/// <summary>
/// Forms, form templates, the PSI form and dashboards carry a version; a save made from an older one is a 409 with the
/// current record and changes nothing (concurrent edits contract).
/// </summary>
public class ConcurrentEditsTests : BaseApiTestFixture
{
    private static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web);

    public record VersionedResponse(Guid Id, uint Version, DateTime? LastModifiedOn, string? LastModifiedByName);

    public record ConflictResponse(string Title, int Status, JsonElement Current);

    private ScenarioData Scenario() =>
        ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithMonitoringNgo(ScenarioNgos.Alfa))
            .Please();

    private static JsonObject Body(object value, uint? version)
    {
        var body = JsonSerializer.SerializeToNode(value, Web)!.AsObject();
        if (version is not null)
        {
            body["version"] = version;
        }

        return body;
    }

    private static HttpResponseMessage Send(HttpClient client, HttpMethod method, string uri, JsonObject body) =>
        client.SendAsync(new HttpRequestMessage(method, uri) { Content = JsonContent.Create(body) })
            .GetAwaiter().GetResult();

    private static T Read<T>(HttpResponseMessage response) =>
        response.Content.ReadFromJsonAsync<T>(Web).GetAwaiter().GetResult()!;

    private static string Text(HttpResponseMessage response) =>
        response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

    /// <summary>Saves from the current version, then from that older one (409, nothing saved), then without one.</summary>
    private static void SavesFromTheCurrentVersionOnly(
        Func<VersionedResponse> get,
        Func<JsonElement, string> savedValueOf,
        Func<string> getSavedValue,
        Func<string, uint?, HttpResponseMessage> save,
        HttpStatusCode saved)
    {
        var before = get();
        before.Version.Should().NotBe(0u);
        before.LastModifiedByName.Should().NotBeNullOrEmpty();

        // From the current version: saved, a new version.
        var first = save("first", before.Version);
        first.StatusCode.Should().Be(saved, Text(first));
        var afterFirst = get();
        afterFirst.Version.Should().NotBe(before.Version);
        afterFirst.LastModifiedOn.Should().NotBeNull().And.BeAfter(DateTime.UtcNow.AddMinutes(-5));
        getSavedValue().Should().Be("first");

        // From the version before: 409 with the record as its GET returns it, nothing saved.
        var stale = save("stale", before.Version);
        stale.StatusCode.Should().Be(HttpStatusCode.Conflict, Text(stale));
        var conflict = Read<ConflictResponse>(stale);
        conflict.Title.Should().Be("This record was changed by someone else");
        conflict.Status.Should().Be(409);
        var current = conflict.Current.Deserialize<VersionedResponse>(Web)!;
        current.Id.Should().Be(before.Id);
        current.Version.Should().Be(afterFirst.Version);
        current.LastModifiedByName.Should().Be(afterFirst.LastModifiedByName);
        savedValueOf(conflict.Current).Should().Be("first");
        getSavedValue().Should().Be("first");
        get().Version.Should().Be(afterFirst.Version);

        // Without a version: saved as before.
        var unchecked_ = save("unchecked", null);
        unchecked_.StatusCode.Should().Be(saved, Text(unchecked_));
        get().Version.Should().NotBe(afterFirst.Version);
        getSavedValue().Should().Be("unchecked");
    }

    [Test]
    public void Forms_SaveFromTheCurrentVersionOnly_AndStatusChangesChangeTheVersion()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var request = Dummy.Form("CE");
        var created = admin.PostWithResponse<VersionedResponse>($"/api/election-rounds/{er}/forms", request);
        created.Version.Should().NotBe(0u);
        created.LastModifiedByName.Should().NotBeNullOrEmpty();
        var uri = $"/api/election-rounds/{er}/forms/{created.Id}";

        SavesFromTheCurrentVersionOnly(
            () => admin.GetResponse<VersionedResponse>(uri),
            current => current.GetProperty("code").GetString()!,
            () => admin.GetResponse<JsonElement>(uri).GetProperty("code").GetString()!,
            (code, version) =>
            {
                request.Code = code;
                return Send(admin, HttpMethod.Put, uri, Body(request, version));
            },
            HttpStatusCode.NoContent);

        var beforePublish = admin.GetResponse<VersionedResponse>(uri).Version;
        admin.PostWithoutResponse($"{uri}:publish");
        admin.GetResponse<VersionedResponse>(uri).Version.Should().NotBe(beforePublish);
    }

    [Test]
    public void FormTemplates_SaveFromTheCurrentVersionOnly()
    {
        var data = Scenario();
        var platformAdmin = data.PlatformAdmin;
        var request = Dummy.Form($"T-{Guid.NewGuid():N}");
        var created = platformAdmin.PostWithResponse<VersionedResponse>("/api/form-templates", request);
        created.Version.Should().NotBe(0u);
        var uri = $"/api/form-templates/{created.Id}";
        string NameOf(JsonElement template) => template.GetProperty("name").GetProperty("EN").GetString()!;

        SavesFromTheCurrentVersionOnly(
            () => platformAdmin.GetResponse<VersionedResponse>(uri),
            NameOf,
            () => NameOf(platformAdmin.GetResponse<JsonElement>(uri)),
            (name, version) =>
            {
                request.Name = new() { { "EN", name }, { "RO", name } };
                return Send(platformAdmin, HttpMethod.Put, uri, Body(request, version));
            },
            HttpStatusCode.NoContent);
    }

    [Test]
    public void PollingStationInformationForm_SavesFromTheCurrentVersionOnly()
    {
        var data = Scenario();
        var platformAdmin = data.PlatformAdmin;
        var uri = $"/api/election-rounds/{data.ElectionRoundId}/polling-station-information-form";
        var request = Dummy.Form("PSI");
        object Upsert(string defaultLanguage) =>
            new { defaultLanguage, languages = new[] { "EN", "RO" }, questions = request.Questions };

        var created = Send(platformAdmin, HttpMethod.Post, uri, Body(Upsert("EN"), null));
        created.StatusCode.Should().Be(HttpStatusCode.OK, Text(created));
        Read<VersionedResponse>(created).Version.Should().NotBe(0u);

        // The value saved is the default language, the only field the upsert really changes.
        var languages = new Dictionary<string, string> { ["first"] = "RO", ["stale"] = "EN", ["unchecked"] = "EN" };
        string LanguageOf(JsonElement form) => form.GetProperty("defaultLanguage").GetString()!;

        SavesFromTheCurrentVersionOnly(
            () => platformAdmin.GetResponse<VersionedResponse>(uri),
            current => LanguageOf(current) == "RO" ? "first" : "unchecked",
            () => LanguageOf(platformAdmin.GetResponse<JsonElement>(uri)) == "RO" ? "first" : "unchecked",
            (value, version) => Send(platformAdmin, HttpMethod.Post, uri, Body(Upsert(languages[value]), version)),
            HttpStatusCode.OK);

        // The upsert answers with the new version.
        var current = platformAdmin.GetResponse<VersionedResponse>(uri);
        var saved = Read<VersionedResponse>(Send(platformAdmin, HttpMethod.Post, uri, Body(Upsert("RO"), current.Version)));
        saved.Version.Should().NotBe(current.Version).And.Be(platformAdmin.GetResponse<VersionedResponse>(uri).Version);
    }

    [Test]
    public void Dashboards_SaveFromTheCurrentVersionOnly_AndAnswerWithTheNewVersion()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var created = Send(admin, HttpMethod.Post, $"/api/election-rounds/{er}/dashboards",
            Body(new { name = "Election day" }, null));
        created.StatusCode.Should().Be(HttpStatusCode.Created, Text(created));
        var dashboard = Read<VersionedResponse>(created);
        dashboard.Version.Should().NotBe(0u);
        dashboard.LastModifiedByName.Should().NotBeNullOrEmpty();
        var uri = $"/api/election-rounds/{er}/dashboards/{dashboard.Id}";
        string NameOf(JsonElement value) => value.GetProperty("name").GetString()!;

        SavesFromTheCurrentVersionOnly(
            () => admin.GetResponse<VersionedResponse>(uri),
            NameOf,
            () => NameOf(admin.GetResponse<JsonElement>(uri)),
            (name, version) => Send(admin, HttpMethod.Put, uri, Body(new { name }, version)),
            HttpStatusCode.OK);

        var current = admin.GetResponse<VersionedResponse>(uri);
        var saved = Read<VersionedResponse>(Send(admin, HttpMethod.Put, uri, Body(new { name = "Night" }, current.Version)));
        saved.Version.Should().NotBe(current.Version).And.Be(admin.GetResponse<VersionedResponse>(uri).Version);
        admin.GetResponse<JsonElement>($"/api/election-rounds/{er}/dashboards").GetProperty("dashboards")[0]
            .GetProperty("version").GetUInt32().Should().Be(saved.Version);
    }
}
