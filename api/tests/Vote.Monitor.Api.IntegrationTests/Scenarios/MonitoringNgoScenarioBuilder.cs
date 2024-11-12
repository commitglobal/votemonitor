using Vote.Monitor.Api.IntegrationTests.Models;

namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class MonitoringNgoScenarioBuilder
{
    public Guid ElectionRoundId { get; }
    private readonly Dictionary<string, MonitoringNgoFormScenarioBuilder> _forms = new();
    private readonly Dictionary<string, HttpClient> _monitoringObservers = new();
    public readonly Guid MonitoringNgoId;
    public readonly ElectionRoundScenarioBuilder ParentBuilder;
    private readonly HttpClient _platformAdmin;
    public NgoScenarioBuilder NgoScenario { get; }
    public Guid FormId => _forms.First().Value.FormId;
    public CreateFormRequest Form => _forms.First().Value.Form;

    public MonitoringNgoScenarioBuilder(Guid electionRoundId,
        Guid monitoringNgoId,
        ElectionRoundScenarioBuilder parentBuilder,
        HttpClient platformAdmin,
        NgoScenarioBuilder ngoScenario)
    {
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
        ParentBuilder = parentBuilder;
        _platformAdmin = platformAdmin;
        NgoScenario = ngoScenario;
    }

    public MonitoringNgoScenarioBuilder WithForm(string? formCode = null,
        Action<MonitoringNgoFormScenarioBuilder>? cfAction = null)
    {
        formCode ??= Guid.NewGuid().ToString();
        var formRequest = Dummy.Form();
        var admin = NgoScenario.Admin;

        var ngoForm = admin.PostWithResponse<CreateFormRequest>(
                $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/forms",
                formRequest);

        admin
            .PostAsync($"/api/election-rounds/{ParentBuilder.ElectionRoundId}/forms/{ngoForm.Id}:publish",
                null)
            .GetAwaiter().GetResult()
            .EnsureSuccessStatusCode();

        var monitoringNgoFormScenarioBuilder = new MonitoringNgoFormScenarioBuilder(this, ngoForm);
        cfAction?.Invoke(monitoringNgoFormScenarioBuilder);

        _forms.Add(formCode, monitoringNgoFormScenarioBuilder);
        return this;
    }

    public MonitoringNgoScenarioBuilder WithMonitoringObserver(string observerEmail)
    {
        var observer = ParentBuilder.ParentBuilder.ObserverByName(observerEmail);
        var observerId = ParentBuilder.ParentBuilder.ObserverIdByName(observerEmail);

        _platformAdmin
            .PostWithResponse<ResponseWithId>(
                $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/monitoring-ngos/{MonitoringNgoId}/monitoring-observers"
                , new { observerId = observerId });

        _monitoringObservers.Add(observerEmail, observer);
        return this;
    }
}
