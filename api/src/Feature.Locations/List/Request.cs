using Vote.Monitor.Core.Models;

namespace Feature.Locations.List;
public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    // Each level filter keeps the records matching any of its values (repeat the key for several).
    [QueryParam]
    public string[]? Level1Filter { get; set; }

    [QueryParam]
    public string[]? Level2Filter { get; set; }

    [QueryParam]
    public string[]? Level3Filter { get; set; }

    [QueryParam]
    public string[]? Level4Filter { get; set; }

    [QueryParam]
    public string[]? Level5Filter { get; set; }
}
