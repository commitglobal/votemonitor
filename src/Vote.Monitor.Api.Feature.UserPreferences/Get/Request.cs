using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.UserPreferences.Get;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid Id { get; set; }
}
