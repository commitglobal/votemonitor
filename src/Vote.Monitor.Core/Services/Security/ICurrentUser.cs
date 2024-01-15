using System.Security.Claims;

namespace Vote.Monitor.Core.Services.Security;

public interface ICurrentUser
{
    Guid GetUserId();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim>? GetUserClaims();
}
