namespace Feature.Review.Inbox.MarkAllAsRead;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }
}
