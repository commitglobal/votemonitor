using SubmissionsFaker.Consts;
using SubmissionsFaker.Extensions;
using SubmissionsFaker.Fakers;
using SubmissionsFaker.Models;

namespace SubmissionsFaker.Scenarios;

public class ElectionRoundScenarioBuilder
{
    public Guid ElectionRoundId { get; }

    private readonly Dictionary<ScenarioPollingStation, Guid> _pollingStations = new();
    private readonly Dictionary<ScenarioNgo, MonitoringNgoScenarioBuilder> _monitoringNgos = new();
    private readonly Dictionary<ScenarioCoalition, CoalitionScenarioBuilder> _coalitions = new();
    private readonly Dictionary<string, Guid> _quickReports = new();

    private readonly HttpClient _platformAdmin;
    public readonly ScenarioBuilder ParentBuilder;

    public ElectionRoundScenarioBuilder WithPollingStation(ScenarioPollingStation pollingStation)
    {
        var createdPollingStation = _platformAdmin.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{ElectionRoundId}/polling-stations",
            new
            {
                Level1 = Guid.NewGuid().ToString(),
                Number = "1",
                DisplayOrder = 1,
                Address = "Address",
                Tags = new { }
            });

        _pollingStations[pollingStation] = createdPollingStation.Id;
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

    public ElectionRoundScenarioBuilder WithMonitoringNgo(ScenarioNgo ngo,
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

    public ElectionRoundScenarioBuilder WithCoalition(ScenarioCoalition name, ScenarioNgo leader, ScenarioNgo[] members,
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

        var response =
            _platformAdmin.GetResponse<ListMonitoringNgos>(
                $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/monitoring-ngos");

        foreach (var monitoringNgo in response.MonitoringNgos)
        {
            var ngo = ParentBuilder.NgoById(monitoringNgo.NgoId);

            var monitoringNgoScenarioBuilder = new MonitoringNgoScenarioBuilder(ElectionRoundId, monitoringNgo.Id, this,
                _platformAdmin,
                ngo.builder);


            _monitoringNgos.TryAdd(ngo.ngo, monitoringNgoScenarioBuilder);
        }

        var coalitionScenarioBuilder =
            new CoalitionScenarioBuilder(_platformAdmin, ParentBuilder.NgoByName(leader).Admin, this, coalition);
        cfg?.Invoke(coalitionScenarioBuilder);

        _coalitions.Add(name, coalitionScenarioBuilder);
        return this;
    }

    public ElectionRoundScenarioBuilder WithQuickReport(ScenarioObserver observer,
        ScenarioPollingStation pollingStation)
    {
        var observerClient = ParentBuilder.ClientFor(observer);
        var pollingStationId = PollingStationByName(pollingStation);

        var quickReport = observerClient.PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{ParentBuilder.ElectionRoundId}/quick-reports",
            new QuickReportRequestFaker(pollingStationId).Generate());

        _quickReports.Add($"{observer}_{pollingStation}", quickReport.Id);
        return this;
    }

    public MonitoringNgoScenarioBuilder MonitoringNgo => _monitoringNgos.First().Value;
    public MonitoringNgoScenarioBuilder MonitoringNgoByName(ScenarioNgo ngo) => _monitoringNgos[ngo];
    public Guid MonitoringNgoIdByName(ScenarioNgo ngo) => _monitoringNgos[ngo].MonitoringNgoId;
    public CoalitionScenarioBuilder Coalition => _coalitions.First().Value;
    public Guid CoalitionId => _coalitions.First().Value.CoalitionId;
    public CoalitionScenarioBuilder CoalitionByName(ScenarioCoalition coalition) => _coalitions[coalition];
    public Guid CoalitionIdByName(ScenarioCoalition coalition) => _coalitions[coalition].CoalitionId;

    public Guid PollingStation => _pollingStations.First().Value;

    public Guid PollingStationByName(ScenarioPollingStation pollingStation) => _pollingStations[pollingStation];

    public Guid GetQuickReportId(ScenarioObserver observer, ScenarioPollingStation pollingStation) =>
        _quickReports[$"{observer}_{pollingStation}"];
}
