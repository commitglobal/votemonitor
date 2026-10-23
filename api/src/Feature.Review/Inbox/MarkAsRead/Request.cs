namespace Feature.Review.Inbox.MarkAsRead;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    public Guid Id { get; set; }
}
