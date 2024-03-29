namespace Vote.Monitor.Api.Feature.PollingStation.FetchAllV3;

public class Response
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public List<PollingStationModelV3> PollingStations { get; set; }
    public List<LevelModel> Level1Values { get; set; } = [];
    public List<LevelModel> Level2Values { get; set; } = [];
    public List<LevelModel> Level3Values { get; set; } = [];
    public List<LevelModel> Level4Values { get; set; } = [];
    public List<LevelModel> Level5Values { get; set; } = [];
}
