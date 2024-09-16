using Vote.Monitor.Core.Security;

namespace Feature.Notifications.MarkAsRead;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public List<Guid> NotificationIds { get; set; } = [];
}