using Feature.NgoCoalitions.Models;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;

namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class CoalitionScenarioBuilder
{
    private readonly HttpClient _platformAdmin;
    private readonly HttpClient _coalitionLeaderAdminAdmin;
    public readonly ElectionRoundScenarioBuilder ParentBuilder;
    private readonly CoalitionModel _coalition;
    private readonly Dictionary<string, CoalitionFormScenarioBuilder> _forms = new();

    public CoalitionScenarioBuilder(HttpClient platformAdmin, HttpClient coalitionLeaderAdmin,
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

        var formRequest = Dummy.Form("A");
        var ngoForm =
            _coalitionLeaderAdminAdmin.PostWithResponse<CreateFormRequest>(
                $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/forms",
                formRequest);

        _coalitionLeaderAdminAdmin
            .PostAsync($"/api/election-rounds/{ParentBuilder.ElectionRoundId}/forms/{ngoForm.Id}:publish",
                null)
            .GetAwaiter().GetResult()
            .EnsureSuccessStatusCode();
        
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

    public CreateFormRequest Form => _forms.First().Value.Form;
    public CoalitionFormScenarioBuilder FormData => _forms.First().Value;
    public Guid FormId => _forms.First().Value.FormId;
    public CreateFormRequest FormByCode(string formCode) => _forms[formCode].Form;
}
