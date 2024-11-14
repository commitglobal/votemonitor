using Vote.Monitor.Api.IntegrationTests.Consts;

namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class ScenarioData
{
    public HttpClient PlatformAdmin { get; }
    private readonly IReadOnlyDictionary<ScenarioElectionRound, ElectionRoundScenarioBuilder> _electionRounds;
    private readonly IReadOnlyDictionary<ScenarioNgo, NgoScenarioBuilder> _ngos;

    private readonly IReadOnlyDictionary<ScenarioObserver, (Guid Id, HttpClient Client, string FullName, string Email,
        string PhoneNumber)> _observers;

    public ScenarioData(HttpClient platformAdmin,
        IReadOnlyDictionary<ScenarioElectionRound, ElectionRoundScenarioBuilder> electionRounds,
        IReadOnlyDictionary<ScenarioNgo, NgoScenarioBuilder> ngos,
        IReadOnlyDictionary<ScenarioObserver, (Guid Id, HttpClient Client, string FullName, string Email, string
            PhoneNumber)> observers)
    {
        PlatformAdmin = platformAdmin;
        _electionRounds = electionRounds;
        _ngos = ngos;
        _observers = observers;
    }

    public ElectionRoundScenarioBuilder ElectionRound => _electionRounds.First().Value;
    public Guid ElectionRoundId => _electionRounds.First().Value.ElectionRoundId;

    public ElectionRoundScenarioBuilder ElectionRoundByName(ScenarioElectionRound electionRound) =>
        _electionRounds[electionRound];

    public Guid ElectionRoundIdByName(ScenarioElectionRound electionRound) =>
        _electionRounds[electionRound].ElectionRoundId;

    public HttpClient Observer => _observers.First().Value.Client;
    public Guid ObserverId => _observers.First().Value.Id;

    public HttpClient ObserverByName(ScenarioObserver observer) => _observers[observer].Client;
    public Guid ObserverIdByName(ScenarioObserver observer) => _observers[observer].Id;

    public NgoScenarioBuilder Ngo => _ngos.First().Value;
    public Guid NgoId => _ngos.First().Value.NgoId;
    public NgoScenarioBuilder NgoByName(ScenarioNgo ngo) => _ngos[ngo];
    public HttpClient AdminOfNgo(ScenarioNgo name) => _ngos[name].Admin;
    public Guid NgoIdByName(ScenarioNgo name) => _ngos[name].NgoId;
}
