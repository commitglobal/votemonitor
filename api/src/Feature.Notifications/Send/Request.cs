using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notifications.Send;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    public string Title { get; set; }
    public string Body { get; set; }
    public string? SearchText { get; set; }
    public string? Level1Filter { get; set; }
    public string? Level2Filter { get; set; }
    public string? Level3Filter { get; set; }
    public string? Level4Filter { get; set; }
    public string? Level5Filter { get; set; }
    public MonitoringObserverStatus? StatusFilter { get; set; }
    public string[]? TagsFilter { get; set; }
}
