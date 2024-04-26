using Vote.Monitor.Core.Security;

namespace Feature.Notes.Delete;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid Id { get; set; }
}
