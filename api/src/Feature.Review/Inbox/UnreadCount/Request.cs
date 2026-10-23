namespace Feature.Review.Inbox.UnreadCount;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }
}

public record Response(int Count);
