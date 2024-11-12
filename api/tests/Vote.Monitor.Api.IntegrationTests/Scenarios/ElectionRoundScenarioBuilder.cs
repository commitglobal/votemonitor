using Feature.NgoCoalitions.Models;
using Vote.Monitor.Api.IntegrationTests.Models;

namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class ElectionRoundScenarioBuilder
{
    public Guid ElectionRoundId { get; }

    private readonly Dictionary<string, Guid> _pollingStations = new();
    private readonly Dictionary<string, MonitoringNgoScenarioBuilder> _monitoringNgos = new();
    private readonly Dictionary<string, CoalitionScenarioBuilder> _coalitions = new();

    private readonly HttpClient _platformAdmin;
    public readonly ScenarioBuilder ParentBuilder;

    public ElectionRoundScenarioBuilder WithPollingStation(string name)
    {
        var pollingStation = _platformAdmin.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{ElectionRoundId}/polling-stations",
            new
            {
                Level1 = Guid.NewGuid().ToString(),
                Number = "1",
                DisplayOrder = 1,
                Address = "Address",
                Tags = new { }
            });

        _pollingStations[name] = pollingStation.Id;
        return this;
    }

    public ElectionRoundScenarioBuilder(ScenarioBuilder parentBuilder,
        Guid electionRoundId,
        HttpClient platformAdmin)
    {
        ParentBuilder = parentBuilder;
        ElectionRoundId = electionRoundId;
        _platformAdmin = platformAdmin;
    }

    public ElectionRoundScenarioBuilder WithMonitoringNgo(string ngo,
        Action<MonitoringNgoScenarioBuilder>? cfg = null)
    {
        var monitoringNgo = _platformAdmin.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{ElectionRoundId}/monitoring-ngos",
            new { ngoId = ParentBuilder.NgoIdByName(ngo) });

        var monitoringNgoScenarioBuilder = new MonitoringNgoScenarioBuilder(ElectionRoundId, monitoringNgo.Id, this,
            _platformAdmin,
            ParentBuilder.NgoByName(ngo));

        cfg?.Invoke(monitoringNgoScenarioBuilder);

        _monitoringNgos.Add(ngo, monitoringNgoScenarioBuilder);

        return this;
    }

    public ElectionRoundScenarioBuilder WithCoalition(string name, string leader, string[] members,
        Action<CoalitionScenarioBuilder>? cfg = null)
    {
        var coalition = _platformAdmin.PostWithResponse<CoalitionModel>(
            $"/api/election-rounds/{ElectionRoundId}/coalitions",
            new
            {
                CoalitionName = Guid.NewGuid().ToString(),
                LeaderId = ParentBuilder.NgoIdByName(leader),
                NgoMembersIds = members.Select(member => ParentBuilder.NgoIdByName(member)).ToArray(),
            });

        var coalitionScenarioBuilder = new CoalitionScenarioBuilder(_platformAdmin, ParentBuilder.NgoByName(leader).Admin, this, coalition);
        cfg?.Invoke(coalitionScenarioBuilder);
        _coalitions.Add(name, coalitionScenarioBuilder);
        return this;
    }

    public MonitoringNgoScenarioBuilder MonitoringNgo => _monitoringNgos.First().Value;
    public MonitoringNgoScenarioBuilder MonitoringNgoByName(string name) => _monitoringNgos[name];
    public Guid MonitoringNgoIdByName(string name) => _monitoringNgos[name].MonitoringNgoId;
    public CoalitionScenarioBuilder Coalition => _coalitions.First().Value;
    public Guid CoalitionId => _coalitions.First().Value.CoalitionId;
    public CoalitionScenarioBuilder CoalitionByName(string name) => _coalitions[name];
    public Guid CoalitionIdByName(string name) => _coalitions[name].CoalitionId;

    public Guid PollingStation => _pollingStations.First().Value;

    public Guid PollingStationByName(string pollingStationName) => _pollingStations[pollingStationName];
}
