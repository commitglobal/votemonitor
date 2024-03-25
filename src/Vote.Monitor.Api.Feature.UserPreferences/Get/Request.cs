using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.UserPreferences.Get;

public class Request
{
    [FromClaim(ClaimTypes.UserId)]
    public Guid Id { get; set; }
}
