using Vote.Monitor.Core.Security;

namespace Feature.UserPreferences.GetMe;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid Id { get; set; }
}
