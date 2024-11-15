using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Fakers;
using Vote.Monitor.Api.IntegrationTests.Models;

namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class MonitoringNgoFormScenarioBuilder
{
    public readonly MonitoringNgoScenarioBuilder ParentBuilder;

    private readonly CreateFormRequest _form;
    public Guid FormId => _form.Id;
    public CreateFormRequest Form => _form;

    private bool _formIsPublished  = false;

    public MonitoringNgoFormScenarioBuilder(
        MonitoringNgoScenarioBuilder parentBuilder,
        CreateFormRequest form)
    {
        ParentBuilder = parentBuilder;
        _form = form;
    }


    public MonitoringNgoFormScenarioBuilder Publish(string? adminEmail = null)
    {
        var admin = adminEmail is not null
            ? ParentBuilder.NgoScenario.AdminByName(adminEmail)
            : ParentBuilder.NgoScenario.Admin;
        
        _formIsPublished = true;

        admin
            .PostAsync($"/api/election-rounds/{ParentBuilder.ElectionRoundId}/forms/{_form.Id}:publish", null)
            .GetAwaiter().GetResult()
            .EnsureSuccessStatusCode();

        return this;
    }

    public MonitoringNgoFormScenarioBuilder WithSubmission(ScenarioObserver observer,
        ScenarioPollingStation pollingStation)
    {
        if (!_formIsPublished)
        {
            throw new ArgumentException("Form is not published");
        }
        
        var pollingStationId = ParentBuilder.ParentBuilder.PollingStationByName(pollingStation);
        var submission = new FakeSubmission(_form.Id, pollingStationId, _form.Questions).Generate();

        var observerClient = ParentBuilder.ParentBuilder.ParentBuilder.ClientFor(observer);

        observerClient.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/form-submissions",
            submission);
        return this;
    }
}
