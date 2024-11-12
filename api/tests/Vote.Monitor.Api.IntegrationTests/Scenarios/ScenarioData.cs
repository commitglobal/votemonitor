namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class ScenarioData
{
    public HttpClient PlatformAdmin { get; }
    private IReadOnlyDictionary<string, ElectionRoundScenarioBuilder> _electionRounds;
    private IReadOnlyDictionary<string, NgoScenarioBuilder> _ngos;
    private IReadOnlyDictionary<string, (Guid Id, HttpClient Client)> _observers;

    public ScenarioData(HttpClient platformAdmin, IReadOnlyDictionary<string, ElectionRoundScenarioBuilder> electionRounds,
        IReadOnlyDictionary<string, NgoScenarioBuilder> ngos,
        IReadOnlyDictionary<string, (Guid Id, HttpClient Client)> observers)
    {
        PlatformAdmin = platformAdmin;
        _electionRounds = electionRounds;
        _ngos = ngos;
        _observers = observers;
    }

    public ElectionRoundScenarioBuilder ElectionRound => _electionRounds.First().Value;
    public Guid ElectionRoundId => _electionRounds.First().Value.ElectionRoundId;
    
    public ElectionRoundScenarioBuilder ElectionRoundByName(string name) => _electionRounds[name];
    public Guid ElectionRoundIdByName(string name) => _electionRounds[name].ElectionRoundId;
    public HttpClient Observer => _observers.First().Value.Client;
    public Guid ObserverId => _observers.First().Value.Id;
    
    public HttpClient ObserverByName(string name) => _observers[name].Client;
    public Guid ObserverIdByName(string name) => _observers[name].Id;
    
    public NgoScenarioBuilder Ngo => _ngos.First().Value;
    public Guid NgoId => _ngos.First().Value.NgoId;
    public NgoScenarioBuilder NgoByName(string name) => _ngos[name];
    public HttpClient AdminOfNgo(string name) => _ngos[name].Admin;
    public Guid NgoIdByName(string name) => _ngos[name].NgoId;
}
