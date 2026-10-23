using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Fakers;
using Vote.Monitor.Api.IntegrationTests.Models;

namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class MonitoringNgoFormScenarioBuilder
{
    public readonly MonitoringNgoScenarioBuilder ParentBuilder;

    private readonly CreateFormRequest _form;
    private readonly Dictionary<string, Guid> _submissions = new();
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
        var submission = new SubmissionFaker(_form.Id, pollingStationId, _form.Questions).Generate();

        var observerClient = ParentBuilder.ParentBuilder.ParentBuilder.ClientFor(observer);

        var submissionId = observerClient.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/form-submissions",
            submission).Id;

        _submissions.Add($"{observer}_{pollingStation}", submissionId);
        return this;
    }

    public Guid GetSubmissionId(ScenarioObserver observer, ScenarioPollingStation pollingStation) =>
        _submissions[$"{observer}_{pollingStation}"];
}
