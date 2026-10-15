using Feature.Statistics.GetNgoAdminStatistics.Models;
using Vote.Monitor.Core.Models;

namespace Feature.Statistics.GetMonitoringObserverLevelStatistics;

public class Response : CachedResponse
{
    public List<LevelStatistics> Levels { get; set; } = [];
}

public class LevelStatistics
{
    public int Level { get; set; }
    public List<VisitedPollingStationLevelStats> Stats { get; set; } = [];
}
