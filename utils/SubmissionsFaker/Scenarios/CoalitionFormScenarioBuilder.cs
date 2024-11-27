using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Consts;
using SubmissionsFaker.Extensions;
using SubmissionsFaker.Fakers;
using SubmissionsFaker.Models;

namespace SubmissionsFaker.Scenarios;

public class CoalitionFormScenarioBuilder
{
    private readonly CoalitionScenarioBuilder _parentBuilder;
    private readonly UpdateFormResponse _form;

    private readonly Dictionary<string, Guid> _submissions = new Dictionary<string, Guid>();

    internal CoalitionFormScenarioBuilder(
        CoalitionScenarioBuilder parentBuilder,
        UpdateFormResponse form)
    {
        _parentBuilder = parentBuilder;
        _form = form;
    }

    public CoalitionFormScenarioBuilder WithSubmission(ScenarioObserver observer, ScenarioPollingStation pollingStation)
    {
        var pollingStationId = _parentBuilder.ParentBuilder.PollingStationByName(pollingStation);
        var submission = new SubmissionFaker(_form.Id, pollingStationId, _form.Questions).Generate();

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
    public UpdateFormResponse Form => _form;
}
