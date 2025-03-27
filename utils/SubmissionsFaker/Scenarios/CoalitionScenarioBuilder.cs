using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Consts;
using SubmissionsFaker.Extensions;
using SubmissionsFaker.Forms;
using SubmissionsFaker.Models;

namespace SubmissionsFaker.Scenarios;

public class CoalitionScenarioBuilder
{
    private readonly HttpClient _coalitionLeaderAdminAdmin;
    public readonly ElectionRoundScenarioBuilder ParentBuilder;
    private readonly CoalitionModel _coalition;
    private readonly Dictionary<string, CoalitionFormScenarioBuilder> _forms = new();
    private readonly Dictionary<string, Guid> _guides = new();

    public CoalitionScenarioBuilder(HttpClient coalitionLeaderAdmin,
        ElectionRoundScenarioBuilder parentBuilder,
        CoalitionModel coalition)
    {
        _coalitionLeaderAdminAdmin = coalitionLeaderAdmin;
        ParentBuilder = parentBuilder;
        _coalition = coalition;
    }

    public Guid CoalitionId => _coalition.Id;

    public CoalitionScenarioBuilder WithForm(string? formCode = null,
        ScenarioNgo[]? sharedWithMembers = null,
        Action<CoalitionFormScenarioBuilder>? cfg = null)
    {
        sharedWithMembers ??= Array.Empty<ScenarioNgo>();
        formCode ??= Guid.NewGuid().ToString();

        var formRequest = ObservationForms.OpeningForm(formCode);
        var ngoForm =
            _coalitionLeaderAdminAdmin.PostWithResponse<CreateFormRequest>(
                $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/forms",
                formRequest);

        _coalitionLeaderAdminAdmin
            .PostWithoutResponse($"/api/election-rounds/{ParentBuilder.ElectionRoundId}/forms/{ngoForm.Id}:publish");

        var members = sharedWithMembers.Select(member => ParentBuilder.ParentBuilder.NgoIdByName(member))
            .ToList();
        _coalitionLeaderAdminAdmin.PutWithoutResponse(
            $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/coalitions/{CoalitionId}/forms/{ngoForm.Id}:access",
            new { NgoMembersIds = members });

        var coalitionFormScenarioBuilder = new CoalitionFormScenarioBuilder(this, ngoForm);
        cfg?.Invoke(coalitionFormScenarioBuilder);

        _forms.Add(formCode, coalitionFormScenarioBuilder);

        return this;
    }

    public CoalitionScenarioBuilder WithMonitoringObserver(ScenarioNgo ngo, ScenarioObserver observer)
    {
        ParentBuilder.ParentBuilder.ElectionRound.MonitoringNgoByName(ngo).WithMonitoringObserver(observer);

        return this;
    }


    public CoalitionScenarioBuilder WithQuickReport(ScenarioObserver observer, ScenarioPollingStation pollingStation)
    {
        var observerClient = ParentBuilder.WithQuickReport(observer, pollingStation);
        return this;
    }


    public CreateFormRequest Form => _forms.First().Value.Form;
    public CoalitionFormScenarioBuilder FormData => _forms.First().Value;
    public Guid FormId => _forms.First().Value.FormId;
    public CreateFormRequest FormByCode(string formCode) => _forms[formCode].Form;

    public Guid GuideId => _guides.First().Value;
    public Guid GuideIdByTitle(string title) => _guides[title];

    public Guid GetSubmissionId(string formCode, ScenarioObserver observer, ScenarioPollingStation pollingStation) =>
        _forms[formCode].GetSubmissionId(observer, pollingStation);
}
