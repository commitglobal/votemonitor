using Vote.Monitor.Core.Security;

namespace Feature.Citizen.Notifications.Send;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    public string Title { get; set; }
    public string Body { get; set; }
}