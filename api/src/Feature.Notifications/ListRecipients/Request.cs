using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.Notifications.ListRecipients;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [QueryParam]
    public string? Level1Filter { get; set; }

    [QueryParam]
    public string? Level2Filter { get; set; }

    [QueryParam]
    public string? Level3Filter { get; set; }

    [QueryParam]
    public string? Level4Filter { get; set; }

    [QueryParam]
    public string? Level5Filter { get; set; }

    [QueryParam]
    public string[] TagsFilter { get; set; }
    public string[] PollingStationNumberFilter { get; set; }
}
