using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Consts;
using SubmissionsFaker.Extensions;
using SubmissionsFaker.Forms;
using SubmissionsFaker.Models;

namespace SubmissionsFaker.Scenarios;

public class MonitoringNgoScenarioBuilder
{
    public Guid ElectionRoundId { get; }
    private readonly Dictionary<string, MonitoringNgoFormScenarioBuilder> _forms = new();
    private readonly Dictionary<ScenarioObserver, (Guid ObserverId, Guid MonitoringObserverId , HttpClient Client, string FullName, string Email,string PhoneNumber)> _monitoringObservers = new();
    public readonly Guid MonitoringNgoId;
    public readonly ElectionRoundScenarioBuilder ParentBuilder;
    private readonly HttpClient _platformAdmin;
    public NgoScenarioBuilder NgoScenario { get; }
    public Guid FormId => _forms.First().Value.FormId;
    public UpdateFormResponse Form => _forms.First().Value.Form;
    public MonitoringNgoFormScenarioBuilder FormDetails => _forms.First().Value;

    public (Guid ObserverId, Guid MonitoringObserverId , HttpClient Client, string FullName, string Email, string PhoneNumber) Observer => _monitoringObservers.First().Value;

    public (Guid ObserverId, Guid MonitoringObserverId, HttpClient Client, string DisplayName, string Email, string PhoneNumber) ObserverByName(ScenarioObserver name) =>
        _monitoringObservers[name];

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
        var formRequest = FormData.OpeningForm(formCode);
        var admin = NgoScenario.Admin;

        var ngoForm = admin.PostWithResponse<UpdateFormResponse>(
                $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/forms",
                formRequest);

        admin
            .PostWithoutResponse($"/api/election-rounds/{ParentBuilder.ElectionRoundId}/forms/{ngoForm.Id}:publish");

        var monitoringNgoFormScenarioBuilder = new MonitoringNgoFormScenarioBuilder(this, ngoForm);
        cfAction?.Invoke(monitoringNgoFormScenarioBuilder);

        _forms.Add(formCode, monitoringNgoFormScenarioBuilder);
        return this;
    }

    public MonitoringNgoScenarioBuilder WithMonitoringObserver(ScenarioObserver observer)
    {
        var observerClient = ParentBuilder.ParentBuilder.ClientFor(observer);
        var observerId = ParentBuilder.ParentBuilder.IdFor(observer);
        var fullName = ParentBuilder.ParentBuilder.FullNameFor(observer);
        var email = ParentBuilder.ParentBuilder.EmailFor(observer);
        var phone = ParentBuilder.ParentBuilder.PhoneNumberFor(observer);

        var monitoringObserver = _platformAdmin
            .PostWithResponse<ResponseWithId>(
                $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/monitoring-ngos/{MonitoringNgoId}/monitoring-observers"
                , new { observerId = observerId });

        _monitoringObservers.Add(observer, (observerId,monitoringObserver.Id, observerClient, fullName, email,phone));
        return this;
    }
    
   
}
