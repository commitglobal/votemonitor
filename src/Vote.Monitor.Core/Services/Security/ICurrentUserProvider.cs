using System.Security.Claims;

namespace Vote.Monitor.Core.Services.Security;

public interface ICurrentUserProvider
{
    Guid? GetUserId();
    Guid? GetNgoId();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim>? GetUserClaims();
    bool IsPlatformAdmin();
    bool IsNgoAdmin();
    bool IsObserver();

}
