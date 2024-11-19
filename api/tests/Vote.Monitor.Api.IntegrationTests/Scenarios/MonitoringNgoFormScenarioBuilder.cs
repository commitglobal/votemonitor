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

    public MonitoringNgoFormScenarioBuilder(
        MonitoringNgoScenarioBuilder parentBuilder,
        CreateFormRequest form)
    {
        ParentBuilder = parentBuilder;
        _form = form;
    }
    
    public MonitoringNgoFormScenarioBuilder WithSubmission(ScenarioObserver observer,
        ScenarioPollingStation pollingStation)
    { 
        var pollingStationId = ParentBuilder.ParentBuilder.PollingStationByName(pollingStation);
        var submission = new FakeSubmission(_form.Id, pollingStationId, _form.Questions).Generate();

        var observerClient = ParentBuilder.ParentBuilder.ParentBuilder.ClientFor(observer);

        observerClient.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/form-submissions",
            submission);
        return this;
    }
}
