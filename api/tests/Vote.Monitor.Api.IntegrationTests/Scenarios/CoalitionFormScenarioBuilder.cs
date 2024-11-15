using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Fakers;
using Vote.Monitor.Api.IntegrationTests.Models;

namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class CoalitionFormScenarioBuilder
{
    private readonly CoalitionScenarioBuilder _parentBuilder;
    private readonly CreateFormRequest _form;

    private readonly Dictionary<string, Guid> _submissions = new Dictionary<string, Guid>();

    internal CoalitionFormScenarioBuilder(
        CoalitionScenarioBuilder parentBuilder,
        CreateFormRequest form)
    {
        _parentBuilder = parentBuilder;
        _form = form;
    }

    public CoalitionFormScenarioBuilder WithSubmission(ScenarioObserver observer, ScenarioPollingStation pollingStation)
    {
        var pollingStationId = _parentBuilder.ParentBuilder.PollingStationByName(pollingStation);
        var submission = new FakeSubmission(_form.Id, pollingStationId, _form.Questions).Generate();

        var observerClient = _parentBuilder.ParentBuilder.ParentBuilder.ClientFor(observer);

        var submissionId = observerClient.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_parentBuilder.ParentBuilder.ElectionRoundId}/form-submissions",
            submission).Id;

        _submissions.Add($"{observer}_{pollingStation}", submissionId);
        return this;
    }

    public Guid GetSubmissionId(ScenarioObserver observer, ScenarioPollingStation pollingStation) =>
        _submissions[$"{observer}_{pollingStation}"];

    public Guid FormId => _form.Id;
    public CreateFormRequest Form => _form;
}
