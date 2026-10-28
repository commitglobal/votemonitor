using System.Text.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.Features.ListFilters;

using static ApiTesting;

/// <summary>Searching forms and form templates matches their name in every language, not only the default one.</summary>
public class FormSearchTests : BaseApiTestFixture
{
    private static CreateFormRequest TranslatedForm(string code)
    {
        var form = Dummy.Form(code);
        form.DefaultLanguage = "EN";
        form.Name = new TranslatedString { { "EN", "asdasd" }, { "RO", "kdjsfkjsdjfk" } };
        return form;
    }

    [Test]
    public void NgoFormsList_ShouldMatchNameInAnyLanguage()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithMonitoringNgo(ScenarioNgos.Alfa))
            .Please();

        var admin = scenarioData.AdminOfNgo(ScenarioNgos.Alfa);
        var url = $"/api/election-rounds/{scenarioData.ElectionRoundId}/forms";
        admin.PostWithResponse<CreateFormRequest>(url, TranslatedForm("F1"));

        Count(admin, url + "?searchText=asdasd").Should().Be(1);
        Count(admin, url + "?searchText=kdjsfkjsdjfk").Should().Be(1);
        Count(admin, url + "?searchText=kdjsf").Should().Be(1);
        Count(admin, url + "?searchText=nothing-like-it").Should().Be(0);
    }

    [Test]
    public void FormTemplatesList_ShouldMatchNameInAnyLanguage()
    {
        var scenarioData = ScenarioBuilder.New(CreateClient).Please();

        var url = "/api/form-templates";
        scenarioData.PlatformAdmin.PostWithResponse<JsonElement>(url, TranslatedForm("T1"));

        Count(scenarioData.PlatformAdmin, url + "?searchText=asdasd").Should().Be(1);
        Count(scenarioData.PlatformAdmin, url + "?searchText=kdjsfkjsdjfk").Should().Be(1);
        Count(scenarioData.PlatformAdmin, url + "?searchText=kdjsf").Should().Be(1);
        Count(scenarioData.PlatformAdmin, url + "?searchText=nothing-like-it").Should().Be(0);
    }

    private static int Count(HttpClient client, string url) =>
        client.GetResponse<PagedResponse<JsonElement>>(url).TotalCount;
}
