
using System.Security.Claims;

namespace Vote.Monitor.Core.Services.Security;

public interface ICurrentUserProvider
{
    ClaimsPrincipal User { get; }
    Guid? GetUserId();
    Guid? GetNgoId();
    bool IsPlatformAdmin();
    bool IsNgoAdmin();
    bool IsObserver();
}
