using Vote.Monitor.Api.Feature.Ngo;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Domain.Constants;

namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class ScenarioBuilder
{
    public HttpClient PlatformAdmin { get; }
    private readonly Func<HttpClient> _clientFactory;
    private readonly Dictionary<string, ElectionRoundScenarioBuilder> _electionRounds  = new();
    private readonly Dictionary<string, NgoScenarioBuilder> _ngos  = new();
    private readonly Dictionary<string, (Guid Id, HttpClient Client)> _observers  = new();

    
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
    public Guid NgoIdByName(string name) => _ngos[name].NgoId;
    
    public static ScenarioBuilder New(Func<HttpClient> clientFactory)
    {
        return new ScenarioBuilder(clientFactory);
    }

    private ScenarioBuilder(Func<HttpClient> clientFactory)
    {
        PlatformAdmin = clientFactory.NewForAuthenticatedUser(CustomWebApplicationFactory.AdminEmail,
            CustomWebApplicationFactory.AdminPassword);
        _clientFactory = clientFactory;
    }

    public ScenarioBuilder WithNgo(string ngoName, Action<NgoScenarioBuilder>? cfg = null)
    {
        var ngo = PlatformAdmin.PostWithResponse<NgoModel>("/api/ngos", new { name = Guid.NewGuid().ToString() });

        var ngoScenarioBuilder = new NgoScenarioBuilder(PlatformAdmin, _clientFactory, ngo.Id);
        ngoScenarioBuilder.WithAdmin();

        cfg?.Invoke(ngoScenarioBuilder);
        _ngos.Add(ngoName, ngoScenarioBuilder);
        return this;
    }

    public ScenarioBuilder WithElectionRound(string name)
    {
        var electionRound = PlatformAdmin.PostWithResponse<ResponseWithId>("/api/election-rounds",
            new
            {
                CountryId = CountriesList.RO.Id,
                Title = name + Guid.NewGuid(),
                EnglishTitle = name,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(69)
            });


        var electionRoundScenarioBuilder =
            new ElectionRoundScenarioBuilder(this, electionRound.Id, PlatformAdmin);

        _electionRounds.Add(name, electionRoundScenarioBuilder);

        return this;
    }

    public ScenarioBuilder WithElectionRound(string name, Action<ElectionRoundScenarioBuilder> cfg)
    {
        var electionRound = PlatformAdmin.PostWithResponse<ResponseWithId>("/api/election-rounds",
            new
            {
                countryId = CountriesList.RO.Id,
                title = name + Guid.NewGuid(),
                englishTitle = name,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(69)
            });


        var electionRoundScenarioBuilder =
            new ElectionRoundScenarioBuilder(this, electionRound.Id, PlatformAdmin);

        cfg(electionRoundScenarioBuilder);

        _electionRounds.Add(name, electionRoundScenarioBuilder);

        return this;
    }

    public ScenarioBuilder WithObserver(string observerEmail)
    {
        var realEmail = $"{Guid.NewGuid()}@example.org";
        var observer = PlatformAdmin.PostWithResponse<ResponseWithId>("/api/observers",
            new { FirstName = "Observer", LastName = observerEmail, Email = realEmail, Password = "string" });

        var observerClient = _clientFactory.NewForAuthenticatedUser(realEmail, "string");

        _observers.Add(observerEmail, (observer.Id, observerClient));
        return this;
    }

    public ScenarioData Please()
    {
        return new(PlatformAdmin, _electionRounds, _ngos, _observers);
    }
}
