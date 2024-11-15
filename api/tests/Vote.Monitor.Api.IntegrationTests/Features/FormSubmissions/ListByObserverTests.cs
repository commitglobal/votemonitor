using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

public class ListByObserverTests : BaseApiTestFixture
{
    private ScenarioData _scenarioData;
    private Guid _electionRoundId;

    private HttpClient _alfaNgoAdmin;
    private HttpClient _betaNgoAdmin;

    private Guid _alfaFormId;
    private Guid _coalitionFormId;

    private Guid _psIasiId;
    private Guid _psBacauId;
    private Guid _psClujId;

    private List<BaseQuestionRequest> _alfaFormQuestions;
    private List<BaseQuestionRequest> _coalitionFormQuestions;

    [SetUp]
    public void Setup()
    {
        _scenarioData = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithForm("A", form => form.Publish()))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta])
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                ))
            .Please();

        _alfaNgoAdmin = _scenarioData.NgoByName(ScenarioNgos.Alfa).Admin;
        _betaNgoAdmin = _scenarioData.NgoByName(ScenarioNgos.Beta).Admin;

        _electionRoundId = _scenarioData.ElectionRoundId;
        _alfaFormId = _scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormId;
        _coalitionFormId = _scenarioData.ElectionRound.Coalition.FormId;

        _psIasiId = _scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi);
        _psBacauId = _scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Bacau);
        _psClujId = _scenarioData.ElectionRound.PollingStationByName(ScenarioPollingStation.Cluj);

        _alfaFormQuestions = _scenarioData.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).Form.Questions;
        _coalitionFormQuestions = _scenarioData.ElectionRound.Coalition.Form.Questions;
    }

    [Test]
    public void List()
    {
        
    }
}
