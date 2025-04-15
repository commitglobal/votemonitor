using Vote.Monitor.Core.Security;

namespace Feature.UserPreferences.Get;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid Id { get; set; }
}
