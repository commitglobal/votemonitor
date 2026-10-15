using Vote.Monitor.Core.Models;

namespace Feature.PollingStations.FetchLevels;

public class Response : CachedResponse
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public List<LevelNode> Nodes { get; set; } = [];
}
