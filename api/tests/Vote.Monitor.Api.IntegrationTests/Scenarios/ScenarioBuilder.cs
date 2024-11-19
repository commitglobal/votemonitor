using Vote.Monitor.Api.Feature.Ngo;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Domain.Constants;

namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class ScenarioBuilder
{
    public HttpClient PlatformAdmin { get; }
    private readonly Func<HttpClient> _clientFactory;
    private readonly Dictionary<ScenarioElectionRound, ElectionRoundScenarioBuilder> _electionRounds = new();
    private readonly Dictionary<ScenarioNgo, NgoScenarioBuilder> _ngos = new();
    private readonly Dictionary<ScenarioObserver, (Guid Id, HttpClient Client, string FullName, string Email, string PhoneNumber)> _observers = new();

    public ElectionRoundScenarioBuilder ElectionRound => _electionRounds.First().Value;
    public Guid ElectionRoundId => _electionRounds.First().Value.ElectionRoundId;

    public ElectionRoundScenarioBuilder ElectionRoundByName(ScenarioElectionRound electionRound) =>
        _electionRounds[electionRound];

    public Guid ElectionRoundIdByName(ScenarioElectionRound electionRound) =>
        _electionRounds[electionRound].ElectionRoundId;

    public string ObserverEmail => _observers.First().Value.Email;
    public string ObserverFullName => _observers.First().Value.FullName;
    public string ObserverPhoneNumber => _observers.First().Value.PhoneNumber;
    public HttpClient Observer => _observers.First().Value.Client;
    public Guid ObserverId => _observers.First().Value.Id;

    public string FullNameFor(ScenarioObserver observer) => _observers[observer].FullName;
    public HttpClient ClientFor(ScenarioObserver observer) => _observers[observer].Client;
    public Guid IdFor(ScenarioObserver observer) => _observers[observer].Id;
    public string EmailFor(ScenarioObserver observer)=> _observers[observer].Email;
    public string PhoneNumberFor(ScenarioObserver observer)=> _observers[observer].PhoneNumber;
    public NgoScenarioBuilder Ngo => _ngos.First().Value;
    public Guid NgoId => _ngos.First().Value.NgoId;
    public NgoScenarioBuilder NgoByName(ScenarioNgo ngo) => _ngos[ngo];

    public (ScenarioNgo ngo, NgoScenarioBuilder builder) NgoById(Guid ngoId)
    {
        var ngo = _ngos.First(x => x.Value.NgoId == ngoId);

        return (ngo.Key, ngo.Value);
    }

    public Guid NgoIdByName(ScenarioNgo ngo) => _ngos[ngo].NgoId;

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

    public ScenarioBuilder WithNgo(ScenarioNgo ngo, Action<NgoScenarioBuilder>? cfg = null)
    {
        var createdNgo =
            PlatformAdmin.PostWithResponse<NgoModel>("/api/ngos", new { name = $"{ngo}-{Guid.NewGuid()}"});

        var ngoScenarioBuilder = new NgoScenarioBuilder(PlatformAdmin, _clientFactory, createdNgo.Id);
        ngoScenarioBuilder.WithAdmin();

        cfg?.Invoke(ngoScenarioBuilder);
        _ngos.Add(ngo, ngoScenarioBuilder);
        return this;
    }

    public ScenarioBuilder WithElectionRound(ScenarioElectionRound electionRound,
        Action<ElectionRoundScenarioBuilder>? cfg = null)
    {
        var title = "ER" + electionRound + Guid.NewGuid();
        var createdElectionRound = PlatformAdmin.PostWithResponse<ResponseWithId>("/api/election-rounds",
            new
            {
                countryId = CountriesList.RO.Id,
                title = title,
                englishTitle = title,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(100)
            });


        var electionRoundScenarioBuilder =
            new ElectionRoundScenarioBuilder(this, createdElectionRound.Id, PlatformAdmin);
        _electionRounds.Add(electionRound, electionRoundScenarioBuilder);

        cfg?.Invoke(electionRoundScenarioBuilder);

        return this;
    }

    public ScenarioBuilder WithObserver(ScenarioObserver observer)
    {
        var realEmail = $"{Guid.NewGuid()}@example.org";
        var lastName = observer + "-" + Guid.NewGuid();
        var phoneNumber = Guid.NewGuid().ToString("N");
        
        var createdObserver = PlatformAdmin.PostWithResponse<ResponseWithId>("/api/observers",
            new { FirstName = "Observer", LastName = lastName, Email = realEmail, Password = "string" , PhoneNumber= phoneNumber});

        var observerClient = _clientFactory.NewForAuthenticatedUser(realEmail, "string");

        _observers.Add(observer, (createdObserver.Id, observerClient, $"Observer {lastName}", realEmail, phoneNumber));
        return this;
    }

    public ScenarioData Please()
    {
        return new(PlatformAdmin, _electionRounds, _ngos, _observers);
    }


}
