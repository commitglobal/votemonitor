using Vote.Monitor.Core.Models;

namespace Feature.Locations.FetchAll;

public class Response : CachedResponse
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public List<LocationNode> Nodes { get; set; } = [];
}
