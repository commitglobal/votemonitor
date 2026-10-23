namespace Feature.Review.Inbox.List;

public class Request : BasePaginatedRequest
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    [QueryParam]
    public bool? UnreadOnly { get; set; }
}
