using System.Security.Claims;

namespace Vote.Monitor.Core.Services.Security;

public interface ICurrentUserIdProvider
{
    ClaimsPrincipal? User { get; }
    Guid? GetUserId();
    string? GetClaimValue(string type);
}
