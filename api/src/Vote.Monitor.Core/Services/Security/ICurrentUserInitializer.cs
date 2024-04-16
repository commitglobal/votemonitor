using System.Security.Claims;

namespace Vote.Monitor.Core.Services.Security;

public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);
}
