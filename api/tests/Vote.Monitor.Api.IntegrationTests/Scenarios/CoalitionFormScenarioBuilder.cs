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

    public CoalitionFormScenarioBuilder WithSubmission(string observerEmail, string pollingStationName)
    {
        var pollingStationId = _parentBuilder.ParentBuilder.PollingStationByName(pollingStationName);
        var submission = new FormSubmissionRequestFaker(_form.Id, pollingStationId, _form.Questions).Generate();

        var observer = _parentBuilder.ParentBuilder.ParentBuilder.ObserverByName(observerEmail);

        var submissionId = observer.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{_parentBuilder.ParentBuilder.ElectionRoundId}/form-submissions",
            submission).Id;

        _submissions.Add($"{observerEmail}_{pollingStationName}", submissionId);
        return this;
    }

    public Guid GetSubmissionId(string observerEmail, string pollingStationName) =>
        _submissions[$"{observerEmail}_{pollingStationName}"];

    public Guid FormId => _form.Id;
}
