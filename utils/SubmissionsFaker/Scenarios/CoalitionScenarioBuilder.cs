using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Consts;
using SubmissionsFaker.Extensions;
using SubmissionsFaker.Forms;
using SubmissionsFaker.Models;

namespace SubmissionsFaker.Scenarios;

public class CoalitionScenarioBuilder
{
    private readonly HttpClient _platformAdmin;
    private readonly HttpClient _coalitionLeaderAdminAdmin;
    public readonly ElectionRoundScenarioBuilder ParentBuilder;
    private readonly CoalitionModel _coalition;
    private readonly Dictionary<string, CoalitionFormScenarioBuilder> _forms = new();
    
    public CoalitionScenarioBuilder(HttpClient platformAdmin,
        HttpClient coalitionLeaderAdmin,
        ElectionRoundScenarioBuilder parentBuilder,
        CoalitionModel coalition)
    {
        _platformAdmin = platformAdmin;
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

        var formRequest = FormData.OpeningForm(formCode);
        var ngoForm =
            _coalitionLeaderAdminAdmin.PostWithResponse<UpdateFormResponse>(
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
    
    

    public UpdateFormResponse Form => _forms.First().Value.Form;
    public CoalitionFormScenarioBuilder FormDetails => _forms.First().Value;
    public Guid FormId => _forms.First().Value.FormId;
    public UpdateFormResponse FormByCode(string formCode) => _forms[formCode].Form;

    public Guid GetSubmissionId(string formCode, ScenarioObserver observer, ScenarioPollingStation pollingStation) =>
        _forms[formCode].GetSubmissionId(observer, pollingStation);
}
