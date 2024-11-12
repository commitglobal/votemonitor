using Feature.NgoCoalitions.Models;
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

    public CoalitionScenarioBuilder WithForm(string? formCode = null, string[]? sharedWithMembers = null,
        Action<CoalitionFormScenarioBuilder>? cfg = null)
    {
        sharedWithMembers ??= Array.Empty<string>();
        formCode ??= Guid.NewGuid().ToString();
        
        var formRequest = Dummy.Form();
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
        _coalitionLeaderAdminAdmin.PostWithoutResponse(
            $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/coalitions/{CoalitionId}/forms/{ngoForm.Id}:access",
            new
            {
                NgoMembersIds = members
            });

        var coalitionFormScenarioBuilder = new CoalitionFormScenarioBuilder(this, ngoForm);
        cfg?.Invoke(coalitionFormScenarioBuilder);

        _forms.Add(formCode, coalitionFormScenarioBuilder);

        return this;
    }

    public CoalitionScenarioBuilder WithMonitoringObserver(string ngo, string observerEmail)
    {
        var observerId = ParentBuilder.ParentBuilder.ObserverIdByName(observerEmail);

        _platformAdmin
            .PostWithResponse<ResponseWithId>(
                $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/monitoring-ngos/{ParentBuilder.MonitoringNgoIdByName(ngo)}/monitoring-observers",
                new { observerId = observerId });

        return this;
    }

    public CoalitionFormScenarioBuilder Form => _forms.First().Value;
    public Guid FormId => _forms.First().Value.FormId;
    public CoalitionFormScenarioBuilder FormByCode(string formCode) => _forms[formCode];
}
