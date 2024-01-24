using System.Security.Claims;

namespace Vote.Monitor.Core.Services.Security;

public interface ICurrentUserProvider
{
    Guid GetUserId();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim>? GetUserClaims();
}
